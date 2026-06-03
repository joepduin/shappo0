using shappo0.Models;
using shappo0.Repositories;
using shappo0.Repositories.Interfaces;

namespace shappo0.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBillRepository _billRepository;   
        private readonly IFeedbackRepository _feedbackRepository;

        public PaymentService(
            IOrderRepository orderRepository, 
            IOrderLineRepository orderLineRepository, 
            IVisitRepository visitRepository, 
            IPaymentRepository paymentRepository,
            IBillRepository billRepository,            
            IFeedbackRepository feedbackRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _paymentRepository = paymentRepository;
            _billRepository = billRepository;               
            _feedbackRepository = feedbackRepository;
        }

        public PaymentOverview GetPaymentOverview(int tableNumber)
        {
            PaymentOverview viewModel = new PaymentOverview
            {
                TableNumber = tableNumber,
                Items = new List<OrderLine>(),
                TotalAmount = 0,
                TotalPaid = 0,
                RemainingAmount = 0,
                VatLow = 0,
                VatHigh = 0
            };

            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableNumber);

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

            Bill bill = _billRepository.GetBillByVisitId(activeVisit.VisitId);
            if (bill != null)
            {
                decimal totalPaid = _paymentRepository.GetTotalPaidForBill(bill.BillId);
                viewModel.TotalPaid = totalPaid;
                decimal remaining = total - totalPaid;
                if (remaining < 0)
                {
                    viewModel.RemainingAmount = 0;
                }
                else
                {
                    viewModel.RemainingAmount = remaining;
                }
            }
            else
            {
                viewModel.TotalPaid = 0;
                viewModel.RemainingAmount = total;
            }

            return viewModel;
        }

        public bool ProcessPayment(PaymentRequest request)
        {
            decimal subTotal = 0;
            decimal vatLow = 0;
            decimal vatHigh = 0;

            List<Order> visitOrders = _orderRepository.GetOrdersByVisitId(request.VisitId);

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

            Bill bill = _billRepository.GetBillByVisitId(request.VisitId);
            int billId;

            if (bill == null)
            {
                Bill newBill = new Bill(subTotal, total, vatLow, vatHigh, false, request.VisitId);

                billId = _billRepository.SaveBill(newBill);
            }
            else
            {
                billId = bill.BillId;
            }

            decimal totalPaid = _paymentRepository.GetTotalPaidForBill(billId);
            decimal remaining = total - totalPaid;

            decimal expectedAmount;
            if (request.IsEqualSplit && request.SplitCount > 0)
                expectedAmount = total / request.SplitCount;
            else
                expectedAmount = remaining;

            decimal finalPaymentAmount;
            if (request.PaymentAmount > 0)
                finalPaymentAmount = request.PaymentAmount;
            else
                finalPaymentAmount = expectedAmount + request.TipAmount;

            decimal calcTip = finalPaymentAmount - expectedAmount;
            decimal finalTip;

            if (calcTip > 0)
            {
                finalTip = calcTip;
            }
            else
            {
                finalTip = 0;
            }

            Payment payment = new Payment
            {
                PaymentMethod = request.PaymentMethod,
                PaymentAmount = finalPaymentAmount,
                BillId = billId,
                PaymentTip = finalTip
            };

            int paymentId = _paymentRepository.SavePayment(payment);

            if (!string.IsNullOrEmpty(request.FeedbackDescription))
            {
                Feedback feedback = new Feedback
                {
                    Type = request.FeedbackType,
                    Description = request.FeedbackDescription,
                    PaymentId = paymentId
                };

                _feedbackRepository.SaveFeedback(feedback);
            }

            decimal newTotalPaid = totalPaid + finalPaymentAmount;


            bool isFullyPaid = newTotalPaid >= total;

            if (isFullyPaid)
            {
                _billRepository.UpdateBillPaid(billId, true);
                _visitRepository.CloseVisit(request.VisitId);
            }


            return isFullyPaid;
        }

        public bool VerifyPaymentSuccess(int tableNumber)
        {
            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableNumber);

            if (activeVisit != null)
            {
                Bill bill = _billRepository.GetBillByVisitId(activeVisit.VisitId);

                if (bill == null || bill.IsPaid == false)
                {
                    return false;
                }
            }

            return true;
        }
    
    }
}
