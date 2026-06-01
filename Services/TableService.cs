using shappo0.ViewModels;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Enums;

namespace shappo0.Services
{
    public class TableService : ITableService
    {
        private readonly ITablesRepository _tablesRepository;

        public TableService(ITablesRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        public List<Order> GetTableByNumber(int tableNumber)
        {
            Table table = _tablesRepository.GetTableByNumber(tableNumber);

            return table.Orders;
        }

        public List<TableViewModel> GetAllTables()
        {
            List<TableViewModel> tablesViewModels = new List<TableViewModel>();
            List<Table> tables = _tablesRepository.GetAllTables();

            foreach(Table table in tables)
            {
                List<OrderStatus> DrinkLines = new List<OrderStatus>();
                List<OrderStatus> FoodLines = new List<OrderStatus>();

                foreach (Order order in table.Orders)
                {
                    foreach(OrderLine orderLine in order.OrderLines)
                    {
                        if (orderLine.Product.ProductType == ProductType.Drink) DrinkLines.Add(orderLine.Status);
                        else FoodLines.Add(orderLine.Status);
                    }
                }

                tablesViewModels.Add(new(table.TableNumber, table.Status, GetStatus(DrinkLines), GetStatus(FoodLines)));
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

