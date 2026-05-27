using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class OrderService : IOrderService
    {
        private IOrderRepository _orderRepository;
        private IOrderLineRepository _orderLineRepository;
        private IMenuRepository _menuRepository;
        private IVisitRepository _visitRepository;
        

        public OrderService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitRepository visitRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _menuRepository = menuRepository;
        }

        public List<Order> GetOrdersByVisitId(int visitId)
        {
            return _orderRepository.GetOrdersByVisitId(visitId);
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public List<Order> GetRunningOrders()
        {
            return _orderRepository.GetRunningOrders();
        }

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            return _orderLineRepository.GetOrderLinesByOrderNumber(orderNumber);
        }


        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines)
        {
            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableNumber);

            int activeVisitId;

            //Create a new visit if this is the first order for this table
            if (activeVisit == null)
            {
                Visit newVisit = new Visit { TableNumber = tableNumber, StaffId = staffId, IsActive = true };
                activeVisitId = _visitRepository.StartNewVisit(newVisit);
            }
            //Else assign the id of the current visit to activeVisitId
            else
            {
                activeVisitId = activeVisit.VisitId;
            }

            Order newOrder = new Order
            {
                WaitingTimeMinutes = 0,
                TimeCreated = DateTime.Now,
                Status = OrderStatus.Running,
                VisitId = activeVisitId,
                StaffId = staffId
            };

            int newOrderId = _orderRepository.CreateOrder(newOrder);

            foreach(OrderLine line in orderLines)
            {
                line.OrderNumber = newOrderId;   
            }
 
            _orderLineRepository.CreateOrderLines(orderLines);
            _menuRepository.BatchAdjustStock(orderLines);
        }
    }
}
