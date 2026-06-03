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
                TotalPaid = 0,
                RemainingAmount = 0,
                VatLow = 0,
                VatHigh = 0
            };

            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit == null)
            {
                return viewModel;
            }

            viewModel.VisitId = activeVisit.VisitId;

            List<Order> visitOrders = _orderRepository.GetOrdersByVisitId(activeVisit.VisitId);

            if (visitOrders == null || visitOrders.Count == 0)
                return viewModel;

            List<OrderLine> groupedItems = new List<OrderLine>();

            foreach (Order order in visitOrders)
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

            Bill bill = _paymentRepository.GetBillByVisitId(activeVisit.VisitId);
            if (bill != null)
            {
                decimal totalPaid = _paymentRepository.GetTotalPaidForBill(bill.BillId);
                viewModel.TotalPaid = totalPaid;
                decimal remaining = total - totalPaid;
                viewModel.RemainingAmount = remaining < 0 ? 0 : remaining;
            }
            else
            {
                viewModel.TotalPaid = 0;
                viewModel.RemainingAmount = total;
            }

            return viewModel;
        }

        public void ProcessPayment(PaymentSubmissionViewModel submission)
        {
            decimal subTotal = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            List<Order> visitOrders = _orderRepository.GetOrdersByVisitId(submission.VisitId);

            if (visitOrders != null)
            {
                foreach (Order order in visitOrders)
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

            Bill bill = _paymentRepository.GetBillByVisitId(submission.VisitId);
            int billId;

            if (bill == null)
            {
                Bill newBill = new Bill
                {
                    SubTotal = subTotal,
                    Total = total,
                    VatLow = vatLow,
                    VatHigh = vatHigh,
                    IsPaid = false,
                    VisitId = submission.VisitId
                };

                billId = _paymentRepository.SaveBill(newBill);
            }
            else
            {
                billId = bill.BillId;
            }

            decimal totalPaid = _paymentRepository.GetTotalPaidForBill(billId);
            decimal remaining = total - totalPaid;

            decimal expectedAmount;
            if (submission.IsEqualSplit && submission.SplitCount > 0)
                expectedAmount = total / submission.SplitCount;
            else
                expectedAmount = remaining;

            decimal finalPaymentAmount;
            if (submission.PaymentAmount > 0)
                finalPaymentAmount = submission.PaymentAmount;
            else
                finalPaymentAmount = expectedAmount + submission.TipAmount;

            decimal calcTip = finalPaymentAmount - expectedAmount;
            decimal finalTip = calcTip > 0 ? calcTip : 0;

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

            decimal newTotalPaid = totalPaid + finalPaymentAmount;
            if (newTotalPaid >= total)
            {
                _paymentRepository.UpdateBillPaid(billId, true);
                _visitRepository.CloseVisit(submission.VisitId);
            }
        }
    }
}
