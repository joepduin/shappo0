using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IStaffRepository _staffRepository;
        

        public OrderService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitRepository visitRepository, IMenuRepository menuRepository, IStaffRepository staffRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitRepository = visitRepository;
            _menuRepository = menuRepository;
            _staffRepository = staffRepository;
        }

        
        public List<Order> GetOrdersByVisitId(int visitId)
        {
            return _orderRepository.GetOrdersByVisitId(visitId);
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public List<Order> GetRunningOrders(OrderStatus status, OrderStatus? status2)
        {
            return _orderRepository.GetRunningOrders(status, status2);
        }

        public List<Order> GetRunningOrdersByType(ProductType type)
        {
            return _orderRepository.GetRunningOrdersByType(type);
        }

        public List<OrderLine> GetOrderLinesByType(int orderNumber, ProductType type)
        {
            return _orderLineRepository.GetOrderLinesByType(orderNumber, type);
        }

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            return _orderLineRepository.GetOrderLinesByOrderNumber(orderNumber);
        }

        public List<OrderLine> GetActiveOrderLinesByTableNumber(int orderNumber)
        {
            return _orderLineRepository.GetActiveOrderLinesByTableNumber(orderNumber);
        }

        public List<OrderLine> GetGroupedOrderLinesByVisitId(int visitId)
        {
            List<OrderLine> groupedLines = new List<OrderLine>();
            List<Order> orders = _orderRepository.GetOrdersByVisitId(visitId);
            foreach (Order order in orders)
            {
                List<OrderLine> orderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);
                foreach (OrderLine orderLine in orderLines)
                {
                    bool itemExists = false;
                    foreach (OrderLine groupLine in groupedLines)
                    {
                        if (groupLine.Product.ProductId == orderLine.Product.ProductId)
                        {
                            groupLine.Quantity += orderLine.Quantity;
                            itemExists = true;
                            break;
                        }
                    }
                    if (itemExists == false) groupedLines.Add(new() { OrderLineId = orderLine.OrderLineId, Product = orderLine.Product, Quantity = orderLine.Quantity });
                }
            }
            return groupedLines;
        }

        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines)
        {
            Visit? activeVisit = _visitRepository.GetActiveVisitForTable(tableNumber);
            Staff currentStaff = _staffRepository.GetStaff(staffId);

            //Create a new visit if this is the first order for this table
            if (activeVisit == null)
            {
                activeVisit = new Visit { Table = new Table{ TableNumber = tableNumber }, Staff = currentStaff, IsActive = true };
                
                int newVisitId = _visitRepository.StartNewVisit(activeVisit);

                activeVisit.VisitId = newVisitId;
            }
 
            Order newOrder = new Order
            {
                TimeCreated = DateTime.Now,
                Status = OrderStatus.Ordered,
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

        public void AddProductToCart(List<OrderLine> cart, Product selectedProduct)
        {
            //A LINQ expression checks if the productId that is passed already exists in the saves OrderLines
            OrderLine? existingItem = cart.FirstOrDefault(i => i.Product.ProductId == selectedProduct.ProductId);

            //If the product already existed in the cart, then the quanity is increased
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            //Otherwise it creates a new product and adds it to the cart
            else
            {
                OrderLine newOrderLine = new OrderLine
                {
                    Product = selectedProduct,
                    Status = OrderStatus.Ordered,
                    Quantity = 1
                };

                cart.Add(newOrderLine);
            }
        }

        public bool TryIncreaseQuantity(List<OrderLine> cart, int productId)
        {
            OrderLine? selectedLine = cart.FirstOrDefault(i => i.Product.ProductId == productId);
            if (selectedLine != null && (selectedLine.Product.Stock - selectedLine.Quantity) > 0)
            {
                selectedLine.Quantity++;

                //Succesful so return true
                return true;
            }
            else
            {
                //Something failed so return false
                return false;
            }
        }

        public void DecreaseQuantity(List<OrderLine> cart, int productId)
        {
            OrderLine? selectedLine = cart.FirstOrDefault(i => i.Product.ProductId == productId);

            if (selectedLine != null && selectedLine.Quantity >= 2)
            {
                selectedLine.Quantity--;
            }
            else
            {
                if (selectedLine != null)
                {
                    cart.Remove(selectedLine);
                }
            }
        }
        public void RemoveLine(List<OrderLine> cart, int productId)
        {
            OrderLine? selectedLine = cart.FirstOrDefault(i => i.Product.ProductId == productId);
            if (selectedLine != null)
            {
                cart.Remove(selectedLine);
            }
        }

        public void AddNote(List<OrderLine> cart, int productId, string note)
        {
            OrderLine? selectedLine = cart.FirstOrDefault(i => i.Product.ProductId == productId);

            if (selectedLine != null)
            {
                selectedLine.Note = note;
            }
        }

        public void UpdateOrderLineStatus(OrderLine orderLine, OrderStatus status)
        {
            _orderLineRepository.UpdateOrderLineStatus(orderLine.OrderLineId, status);

            List<OrderLine> orderLines = _orderLineRepository.GetOrderLinesByOrderNumber(orderLine.Order.OrderNumber);
            bool allLinesStatus = true;

            foreach (OrderLine newOrderLine in orderLines)
            {
                if (newOrderLine.Status != status) allLinesStatus = false;
            }

            if (allLinesStatus) _orderRepository.UpdateOrderStatus(orderLine.Order.OrderNumber, status);
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _orderRepository.UpdateOrderStatus(orderNumber, status);

            List<OrderLine> orderLines = _orderLineRepository.GetOrderLinesByOrderNumber(orderNumber);
            foreach (OrderLine orderLine in orderLines)
            {
                if (orderLine.Status != status) _orderLineRepository.UpdateOrderLineStatus(orderLine.OrderLineId, status);
            }
        }

        public void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status)
        {
            _orderLineRepository.UpdateCourseStatus(orderNumber, productType, status);
        }
    }
}
