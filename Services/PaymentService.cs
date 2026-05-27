using shappo0.Models;
using shappo0.Repositories;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IVisitRepository _visitRepository;

        
        public PaymentService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitRepository visitRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
        }

        public PaymentViewModel GetPaymentOverview(int tableId)
        {
            //Creates a mostly empty view model to be filled in later
            PaymentViewModel viewModel = new PaymentViewModel
            {
                TableId = tableId,
                Items = new List<OrderLine>(),
                TotalAmount = 0,
                VatLow = 0,
                VatHigh = 0
            };

            //Fetch the active visit for the selected table
            Visit? activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit == null)
            {
                return viewModel;
            }

            List<Order> visitOrders = _orderRepository.GetOrdersByVisitId(activeVisit.VisitId);

            
            //If there aren't any orders for this visit, return empty view
            if (visitOrders == null || visitOrders.Count == 0) return viewModel;
            

            // Groepeer orderregels op product zodat dubbele regels worden samengevoegd
            List<OrderLine> groupedItems = new List<OrderLine>();

            foreach (Order order in visitOrders)
            {
                //Haal de werkelijke orderregels voor deze specifieke order uit de database!
                order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);

                // Zorg ervoor dat er daadwerkelijk items zijn voordat we er doorheen loopen
                if (order.OrderLines != null)
                {
                    foreach (OrderLine item in order.OrderLines)
                    {
                        OrderLine existingItem = null;
                        foreach (OrderLine groupedItem in groupedItems)
                        {
                            if (groupedItem.Product.ProductId == item.Product.ProductId)
                            {
                                existingItem = groupedItem;
                                break;
                            }
                        }

                        // Als product nog niet in gegroepeerde lijst staat, toevoegen
                        if (existingItem == null)
                        {
                            groupedItems.Add(new OrderLine
                            {
                                OrderLineId = item.OrderLineId,
                                Quantity = item.Quantity,
                                Product = item.Product
                            });
                        }
                        else
                        {
                            // Anders tel de hoeveelheden op (samenvoegen van bijv. 2 cola en nog eens 3 cola)
                            existingItem.Quantity += item.Quantity;
                        }
                    }
                }
            }

            viewModel.Items = groupedItems;

            // Bereken totaal en BTW per groep
            decimal total = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            foreach (OrderLine item in groupedItems)
            {
                // Basisprijs per regel = prijs (excl. BTW) * aantal
                decimal lineBase = item.Product.Price * item.Quantity;

                // Bereken BTW als simpele vermenigvuldiging: basis * (VAT% / 100)
                // Dit volgt de manier waarop producten worden opgeslagen in Product.Price (excl. BTW)
                decimal vatAmount = lineBase * (item.Product.VATPercentage / 100m);

                // Tel BTW en basis op bij de totalen
                total += lineBase + vatAmount;

                // Voeg BTW toe aan hoge of lage categorie
                if (item.Product.VATPercentage == 21)
                    vatHigh += vatAmount;
                else
                    vatLow += vatAmount;
            }

            // Sla uitkomsten op in het viewmodel
            viewModel.TotalAmount = total;
            viewModel.VatLow = vatLow;
            viewModel.VatHigh = vatHigh;

            return viewModel;
        }
    }
}
