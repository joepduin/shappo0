using shappo0.Models;
using shappo0.Repositories;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;

        
        public PaymentService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public PaymentViewModel GetPaymentOverview(int tableId)
        {
            // Haal de order op uit de database voor deze tafel
            // Dit roept de repository aan die ADO.NET gebruikt
            Order currentOrder = _orderRepository.GetOrderForTable(tableId);
            PaymentViewModel viewModel = new PaymentViewModel();

            viewModel.TableId = tableId;

            
            // Als er geen order is, geef een leeg overzicht terug
            if (currentOrder == null)
            {
                // Zet lege lijst en nulwaarden zodat de view netjes blijft
                viewModel.Items = new List<OrderLine>();
                viewModel.TotalAmount = 0;
                viewModel.VatLow = 0;
                viewModel.VatHigh = 0;
                return viewModel;
            }

           
            if (currentOrder.OrderItems == null || currentOrder.OrderItems.Count == 0)
            {
                viewModel.Items = new List<OrderLine>();
                viewModel.TotalAmount = 0;
                viewModel.VatLow = 0;
                viewModel.VatHigh = 0;
                return viewModel;
            }

            // Groepeer orderregels op product zodat dubbele regels worden samengevoegd
            List<OrderLine> groupedItems = new List<OrderLine>();
            foreach (OrderLine item in currentOrder.OrderItems)
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
                    // Anders tel de hoeveelheden op
                    existingItem.Quantity += item.Quantity;
                }
            }

            viewModel.Items = groupedItems;

            // Bereken totaal en BTW per groep
            decimal total = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            foreach (OrderLine item in groupedItems)
            {
                // Prijs per regel = prijs product * aantal
                decimal lineTotal = item.Product.Price * item.Quantity;
                total += lineTotal;

                // Bepaal of dit product 21% of 9% BTW heeft
                if (item.Product.VATPercentage == 21)
                {
                    // Bereken hoog BTW deel uit een prijs die inclusief BTW is
                    vatHigh += (lineTotal / 121) * 21;
                }
                else
                {
                    // Bereken laag BTW deel uit een prijs die inclusief BTW is
                    vatLow += (lineTotal / 109) * 9;
                }
            }

            // Sla uitkomsten op in het viewmodel
            viewModel.TotalAmount = total;
            viewModel.VatLow = vatLow;
            viewModel.VatHigh = vatHigh;

            return viewModel;
        }
    }
}
