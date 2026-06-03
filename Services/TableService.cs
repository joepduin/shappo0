using shappo0.ViewModels;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Enums;
using shappo0.ViewModels;

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

        public TableViewModel GetTableByNumber(int tableNumber)
        {
            Table table = _tablesRepository.GetTableByNumber(tableNumber);
            List<Order> orders = new List<Order>();

            Visit? visit = _visitRepository.GetActiveVisitForTable(tableNumber);
            if (visit != null)
            {
                orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
                foreach (Order order in orders)
                {
                    order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);
                }
            }

            return new TableViewModel(table.TableNumber, table.Status, orders);
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

                tablesViewModels.Add(new(table.TableNumber, table.Status, GetStatus(drinkLines), GetStatus(foodLines)));
            }

            return tablesViewModels;
        }

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
    }
}

