using System.Collections.Generic;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Discard
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

        public void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status)
        {
            _kitchenRepository.UpdateCourseStatus(orderNumber, productType, status);
        }

    }
}
