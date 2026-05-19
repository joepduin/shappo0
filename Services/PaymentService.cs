using shappo0.Models;
using shappo0.Repositories.Chapeau.Repositories;
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
            Order currentOrder = _orderRepository.GetOrderForTable(tableId);
            PaymentViewModel viewModel = new PaymentViewModel();

            viewModel.TableId = tableId;

            
            if (currentOrder == null)
            {
               
                viewModel.Items = new List<OrderItem>();
                viewModel.TotalAmount = 0;
                viewModel.VatLow = 0;
                viewModel.VatHigh = 0;
                return viewModel;
            }

           
            viewModel.Items = currentOrder.OrderItems;

            double total = 0;
            double vatLow = 0;
            double vatHigh = 0;

            foreach (OrderItem item in currentOrder.OrderItems)
            {
                double lineTotal = item.MenuItem.Price * item.Count;
                total += lineTotal;

                if (item.MenuItem.IsAlcoholic)
                {
                    vatHigh += (lineTotal / 121) * 21;
                }
                else
                {
                    vatLow += (lineTotal / 109) * 9;
                }
            }

            viewModel.TotalAmount = total;
            viewModel.VatLow = vatLow;
            viewModel.VatHigh = vatHigh;

            return viewModel;
        }
    }
}
