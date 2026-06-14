using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IAccountService _accountService;
        private readonly IMenuService _menuService;
        private readonly IVisitService _visitService;
        

        public OrderService(IOrderRepository orderRepository, IOrderLineRepository orderLineRepository, IVisitService visitService, IMenuService menuService, IAccountService accountService)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _visitService = visitService;
            _menuService = menuService;
            _accountService = accountService;
        }

        
        public List<Order> GetOrdersByVisitId(int visitId)
        {
            return _orderRepository.GetOrdersByVisitId(visitId);
        }

        public List<Order> GetRunningOrdersByType(ProductType type)
        {
            return _orderRepository.GetRunningOrdersByType(type);
        }

        public List<Order> GetFinishedOrdersByType(ProductType type)
        {
            return _orderRepository.GetFinishedOrdersByType(type);
        }

        public List<OrderLine> GetOrderLinesByType(int orderNumber, ProductType type)
        {
            return _orderLineRepository.GetOrderLinesByType(orderNumber, type);
        }

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            return _orderLineRepository.GetOrderLinesByOrderNumber(orderNumber);
        }

        public List<OrderLine> GetGroupedOrderLinesByVisitId(int visitId)
        {
            return _orderLineRepository.GetGroupedOrderLinesByVisitId(visitId);
        }

        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines)
        {
            Visit? activeVisit = _visitService.GetActiveVisitForTable(tableNumber);
            Staff? currentStaff = _accountService.GetStaff(staffId);

            //If the staff member doesn't exist, return without creating an order
            if (currentStaff == null)
            {
                return;
            }

            //Create a new visit if this is the first order for this table
            if (activeVisit == null)
            {
                activeVisit = new Visit { Table = new Table{ TableNumber = tableNumber }, Staff = currentStaff, IsActive = true };
                
                int newVisitId = _visitService.StartNewVisit(activeVisit);

                activeVisit.VisitId = newVisitId;
  
            }

            Order newOrder = new()
            {
                TimeCreated = DateTime.Now,
                Status = OrderStatus.Ordered,
                Visit = activeVisit,
                Staff = currentStaff
            };

            _orderRepository.CreateOrder(newOrder);

            //Link the order lines to the order and create them in the database, then adjust the stock for each ordered product
            foreach (OrderLine line in orderLines)
            {
                line.Order = newOrder;
            }

            _orderLineRepository.CreateOrderLines(orderLines);
            _menuService.BatchAdjustStock(orderLines);
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
                OrderLine newOrderLine = new()
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

        public OrderStatus GetOrderStatusByVisit(Visit visit, ProductType productType)
        {
            List<Order> orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
            List<OrderLine> orderLines = [];

            foreach (Order order in orders)
            {
                orderLines.AddRange(_orderLineRepository.GetOrderLinesByType(order.OrderNumber, productType));
            }

            return orderLines
                .Select(ol => ol.Status)
                .DefaultIfEmpty(OrderStatus.Served)
                .Max();
        }
    }
}
