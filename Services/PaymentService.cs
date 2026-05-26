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
        private readonly IPaymentRepository _paymentRepository;

        
        public PaymentService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitRepository visitRepository, IPaymentRepository paymentRepository)
        {
            // Constructor injection: de benodigde repositories worden door het framework geplaatst.
            // Dit volgt de projectstijl en maakt de service testbaar en los gekoppeld.
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _paymentRepository = paymentRepository;
        }

        public PaymentViewModel GetPaymentOverview(int tableId)
        {
            //Creates a mostly empty view model to be filled in later
            // Uitleg: Deze service bouwt het viewmodel dat de controller aan de view geeft.
            // De service bevat de business logic: ophalen van visit, orders en groeperen van orderregels.
            PaymentViewModel viewModel = new PaymentViewModel
            {
                TableId = tableId,
                Items = new List<OrderLine>(),
                TotalAmount = 0,
                VatLow = 0,
                VatHigh = 0
            };

            //Fetch the active visit for the selected table
            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit == null)
            {
                return viewModel;
            }

            // Leg het visitId vast in het viewmodel zodat de view het kan opnemen
            // Dit is nodig om later bij het opslaan van de bill de juiste visit te koppelen.
            viewModel.VisitId = activeVisit.VisitId;

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

        public void ProcessPayment(int tableId, int visitId, decimal subTotal, decimal vatLow, decimal vatHigh, decimal tipAmount, decimal paymentAmount, string paymentMethod, string feedbackType, string feedbackDescription)
        {
            // Uitleg: Business flow
            // 1) Bereken de bill total en sla de Bill op
            // 2) Bepaal het werkelijke betaalde bedrag en de fooi
            // 3) Sla de Payment op en koppel Feedback indien aanwezig
            // 4) Zet de tafel vrij
            // Deze stappen volgen de Sprint 2 vereisten en gebruiken de repository voor DB-toegang.
            // Bereken totaal van de bill
            decimal total = subTotal + vatLow + vatHigh;

            // Maak bill object en zet isPaid = true
            Bill bill = new Bill
            {
                SubTotal = subTotal,
                Total = total,
                VatLow = vatLow,
                VatHigh = vatHigh,
                IsPaid = true,
                VisitId = visitId
            };

            // Sla bill op en krijg billId terug
            int billId = _paymentRepository.SaveBill(bill);

            // Zorg dat de betaalmethode geldig is, anders default naar cash
            string method = paymentMethod?.ToLower() ?? "cash";
            if (method != "credit card" && method != "debit card" && method != "cash")
            {
                method = "cash";
            }

            // Bepaal payment amount en tip. Als paymentAmount > 0 is gebruikt de gast dit bedrag.
            decimal finalPaymentAmount;
            decimal finalTip;

            if (paymentAmount > 0)
            {
                finalPaymentAmount = paymentAmount;
                decimal calcTip = paymentAmount - total;
                finalTip = calcTip > 0 ? calcTip : 0;
            }
            else
            {
                finalTip = tipAmount;
                finalPaymentAmount = total + tipAmount;
            }

            // Maak payment object
            Payment payment = new Payment
            {
                PaymentMethod = method,
                PaymentAmount = finalPaymentAmount,
                BillId = billId,
                PaymentTip = finalTip
            };

            // Sla payment op en krijg paymentId
            int paymentId = _paymentRepository.SavePayment(payment);

            // Voeg feedback toe indien aanwezig
            if (!string.IsNullOrEmpty(feedbackDescription))
            {
                Feedback feedback = new Feedback
                {
                    Type = feedbackType,
                    Description = feedbackDescription,
                    PaymentId = paymentId
                };

                _paymentRepository.SaveFeedback(feedback);
            }

            // Zet tafel vrij
            _paymentRepository.FreeTable(tableId);
        }
    }
}
