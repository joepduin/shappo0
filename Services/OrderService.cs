using shappo0.Enums;
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
        private IStaffRepository _staffRepository;
        

        public OrderService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitRepository visitRepository, IMenuRepository menuRepository, IStaffRepository staffRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _menuRepository = menuRepository;
            _staffRepository = staffRepository;
        }

        public decimal CalculateFinalPrice(decimal price, decimal VATPercentage)
        {
            return price + (price * VATPercentage/100);
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
            Staff currentStaff = _staffRepository.GetStaff(staffId);

            //Create a new visit if this is the first order for this table
            if (activeVisit == null)
            {
                Visit newVisit = new Visit { Table = new Table{ TableNumber = tableNumber }, Staff = currentStaff, IsActive = true };
                
                int newVisitId = _visitRepository.StartNewVisit(newVisit);

                newVisit.VisitId = newVisitId;
            }
 
            Order newOrder = new Order
            {
                WaitingTimeMinutes = 0,
                TimeCreated = DateTime.Now,
                Status = OrderStatus.Running,
                Visit = activeVisit,
                Staff = currentStaff
            };

            _orderRepository.CreateOrder(newOrder);

            foreach(OrderLine line in orderLines)
            {
                line.Order = newOrder;   
            }
 
            _orderLineRepository.CreateOrderLines(orderLines);
            _menuRepository.BatchAdjustStock(orderLines);
        }
    }
}
