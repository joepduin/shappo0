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

        public TableService(ITablesRepository tablesRepository, IVisitRepository visitRepository, IOrderRepository orderRepository)
        {
            _tablesRepository = tablesRepository;
            _visitRepository = visitRepository;
            _orderRepository = orderRepository;
        }

        public Table GetTableByNumber(int tableNumber)
        {
            Table table = _tablesRepository.GetTableByNumber(tableNumber);
            return table;
        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = _tablesRepository.GetAllTables();
            return tables;
        }

        public bool UpdateTableStatus(Table table)
        {
            Visit? visit = _visitRepository.GetActiveVisitForTable(table.TableNumber);

            if (visit != null)
            {
                List<OrderViewmodel> orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
                if (orders != null)
                {
                    bool allOrdersServed = true;
                    foreach (OrderViewmodel order in orders)
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