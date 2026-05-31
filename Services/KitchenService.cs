using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class KitchenService : IKitchenService
    {
        private readonly IKitchenRepository _kitchenRepository;

        public KitchenService(IKitchenRepository kitchenRepository)
        {
            _kitchenRepository = kitchenRepository;
        }

        public List<Kitchen> GetAll()
        {
            return _kitchenRepository.GetAll();
        }

        public Kitchen? GetById(int OrderNumber)
        {
            return _kitchenRepository.GetById(OrderNumber);

        }

        public List<OrderLine> GetOrderLines(int orderNumber)
        {
            return _kitchenRepository.GetOrderLines(orderNumber);
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _kitchenRepository.UpdateOrderStatus(orderNumber, status);
        }

        public void UpdateOrderLineStatus(int orderNumber, OrderStatus status)
        {
            _kitchenRepository.UpdateOrderLineStatus(orderNumber, status);
        }

       public List<Kitchen> FinishedOrders()
        {
            return _kitchenRepository.FinishedOrders();
        }
    }
}
