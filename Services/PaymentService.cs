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

           
            if (currentOrder.OrderItems == null || currentOrder.OrderItems.Count == 0)
            {
                viewModel.Items = new List<OrderItem>();
                viewModel.TotalAmount = 0;
                viewModel.VatLow = 0;
                viewModel.VatHigh = 0;
                return viewModel;
            }

            List<OrderItem> groupedItems = new List<OrderItem>();
            foreach (OrderItem item in currentOrder.OrderItems)
            {
                OrderItem existingItem = null;
                foreach (OrderItem groupedItem in groupedItems)
                {
                    if (groupedItem.MenuItem.Id == item.MenuItem.Id)
                    {
                        existingItem = groupedItem;
                        break;
                    }
                }

                if (existingItem == null)
                {
                    groupedItems.Add(new OrderItem
                    {
                        Id = item.Id,
                        Count = item.Count,
                        MenuItem = item.MenuItem
                    });
                }
                else
                {
                    existingItem.Count += item.Count;
                }
            }

            viewModel.Items = groupedItems;

            double total = 0;
            double vatLow = 0;
            double vatHigh = 0;

            foreach (OrderItem item in groupedItems)
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
