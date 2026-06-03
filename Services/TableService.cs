using shappo0.ViewModels;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Enums;

namespace shappo0.Services
{
    public class TableService : ITableService
    {
        private readonly ITablesRepository _tablesRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;

        public TableService(ITablesRepository tablesRepository, IVisitRepository visitRepository, IOrderRepository orderRepository, IOrderLineRepository orderLineRepository)
        {
            _tablesRepository = tablesRepository;
            _visitRepository = visitRepository;
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
        }

        public bool GetStatusByNumber(int tableNumber)
        {
            Table table = _tablesRepository.GetTableByNumber(tableNumber);
            return table.Status;
        }

        public TableViewModel GetTableByNumber(int tableNumber)
        {
            TableViewModel tableViewModel = null;
            Table table = _tablesRepository.GetTableByNumber(tableNumber);
            if (table != null)
            {
                Visit? visit = _visitRepository.GetActiveVisitForTable(tableNumber);
                List<Order> orders = new List<Order>();
                if (visit != null)
                {
                    orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
                    foreach (Order order in orders)
                    {
                        order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);
                    }
                }
                tableViewModel = new TableViewModel(table, orders);
            }

            return tableViewModel;
        }

        public List<TableViewModel> GetAllTables()
        {
            List<TableViewModel> tablesViewModels = new List<TableViewModel>();
            List<Table> tables = _tablesRepository.GetAllTables();

            foreach(Table table in tables)
            {
                List<OrderStatus> drinkLines = new List<OrderStatus>();
                List<OrderStatus> foodLines = new List<OrderStatus>();
                List<OrderLine> orderLines = _orderLineRepository.GetActiveOrderLinesByTableNumber(table.TableNumber);

                foreach(OrderLine orderLine in orderLines)
                {
                    if (orderLine.Product.ProductType == ProductType.Drink) drinkLines.Add(orderLine.Status);
                    else foodLines.Add(orderLine.Status);
                }

                tablesViewModels.Add(new(table, GetStatus(drinkLines), GetStatus(foodLines)));
            }

            return tablesViewModels;
        }

        //methode makkelijker maken
        private OrderStatus GetStatus(List<OrderStatus> orderStatuses)
        {
            OrderStatus status = OrderStatus.Served;

            foreach(OrderStatus orderStatus in orderStatuses)
            {
                if (status != orderStatus)
                {
                    if (status == OrderStatus.Served) status = orderStatus;

                    if (status == OrderStatus.Ordered)
                    {
                        if (orderStatus == OrderStatus.BeingPrepared) status = orderStatus;
                        if (orderStatus == OrderStatus.ReadyToBeServed) status = orderStatus;
                    }

                    if (status == OrderStatus.BeingPrepared)
                    {
                        if (orderStatus == OrderStatus.ReadyToBeServed) status = orderStatus;
                    }

                    if (orderStatus == OrderStatus.ReadyToBeServed) status = orderStatus;
                }
            }

            return status;
        }

        public void UpdateOrderLineStatus(int orderLineId, OrderStatus status, int orderNumber)
        {
            _orderLineRepository.UpdateOrderLineStatus(orderLineId, status);

            List<OrderLine> orderLines = _orderLineRepository.GetOrderLinesByOrderNumber(orderNumber);
            bool allLinesServed = true;

            foreach (OrderLine orderLine in orderLines)
            {
                if (orderLine.Status != OrderStatus.Served)
                {
                    allLinesServed = false;
                    break;
                }
            }

            if (allLinesServed) _orderRepository.UpdateOrderStatus(orderNumber, OrderStatus.Served);
        }

        public bool UpdateTableStatus(Table table)
        {
            Visit? visit = _visitRepository.GetActiveVisitForTable(table.TableNumber);

            if (visit != null)
            {
                List<Order> orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
                if (orders != null)
                {
                    bool allOrdersServed = true;
                    foreach (Order order in orders)
                    {
                        if (order.Status != OrderStatus.Served) allOrdersServed = false;
                    }

                    if (allOrdersServed)
                    {
                        _visitRepository.CloseVisit(visit.VisitId);
                        _tablesRepository.ChangeTableStatus(table);
                        return true;
                    }
                }
            }
            else
            {
                _tablesRepository.ChangeTableStatus(table);
                return true;
            }

            return false;
        }
    }
}