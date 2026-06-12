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
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _paymentRepository = paymentRepository;
        }

        public PaymentViewModel GetPaymentOverview(int tableId)
        {
            PaymentViewModel viewModel = new PaymentViewModel
            {
                TableId = tableId,
                Items = new List<OrderLine>(),
                TotalAmount = 0,
                VatLow = 0,
                VatHigh = 0
            };

            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit == null)
            {
                return viewModel;
            }

            viewModel.VisitId = activeVisit.VisitId;

            List<OrderViewmodel> visitOrders = _orderRepository.GetOrdersByVisitId(activeVisit.VisitId);

            if (visitOrders == null || visitOrders.Count == 0)
                return viewModel;

            List<OrderLine> groupedItems = new List<OrderLine>();

            foreach (OrderViewmodel order in visitOrders)
            {
                order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);

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
                            existingItem.Quantity += item.Quantity;
                        }
                    }
                }
            }

            viewModel.Items = groupedItems;

            decimal total = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            foreach (OrderLine item in groupedItems)
            {
                decimal lineBase = item.Product.Price * item.Quantity;
                decimal vatAmount = lineBase * (item.Product.VATPercentage / 100m);

                total += lineBase + vatAmount;

                if (item.Product.VATPercentage == 21)
                    vatHigh += vatAmount;
                else
                    vatLow += vatAmount;
            }

            viewModel.TotalAmount = total;
            viewModel.VatLow = vatLow;
            viewModel.VatHigh = vatHigh;

            return viewModel;
        }

        public void ProcessPayment(PaymentSubmissionViewModel submission)
        {
            decimal subTotal = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            List<OrderViewmodel> visitOrders = _orderRepository.GetOrdersByVisitId(submission.VisitId);

            if (visitOrders != null)
            {
                foreach (OrderViewmodel order in visitOrders)
                {
                    order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);

                    if (order.OrderLines != null)
                    {
                        foreach (OrderLine item in order.OrderLines)
                        {
                            decimal lineBase = item.Product.Price * item.Quantity;
                            decimal vatAmount = lineBase * (item.Product.VATPercentage / 100m);

                            subTotal += lineBase;

                            if (item.Product.VATPercentage == 21)
                                vatHigh += vatAmount;
                            else
                                vatLow += vatAmount;
                        }
                    }
                }
            }

            decimal total = subTotal + vatLow + vatHigh;

            Bill bill = new Bill
            {
                SubTotal = subTotal,
                Total = total,
                VatLow = vatLow,
                VatHigh = vatHigh,
                IsPaid = true,
                VisitId = submission.VisitId
            };

            int billId = _paymentRepository.SaveBill(bill);

            decimal finalPaymentAmount;
            decimal finalTip;

            if (submission.PaymentAmount > 0)
            {
                finalPaymentAmount = submission.PaymentAmount;
                decimal calcTip = submission.PaymentAmount - total;
                finalTip = calcTip > 0 ? calcTip : 0;
            }
            else
            {
                finalTip = submission.TipAmount;
                finalPaymentAmount = total + submission.TipAmount;
            }

            Payment payment = new Payment
            {
                PaymentMethod = submission.PaymentMethod,
                PaymentAmount = finalPaymentAmount,
                BillId = billId,
                PaymentTip = finalTip
            };

            int paymentId = _paymentRepository.SavePayment(payment);

            if (!string.IsNullOrEmpty(submission.FeedbackDescription))
            {
                Feedback feedback = new Feedback
                {
                    Type = submission.FeedbackType,
                    Description = submission.FeedbackDescription,
                    PaymentId = paymentId
                };

                _paymentRepository.SaveFeedback(feedback);
            }

            _visitRepository.CloseVisit(submission.VisitId);
        }
    }
}
