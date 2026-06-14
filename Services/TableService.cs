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

        //Geeft het tafel object
        public Table? GetTableByNumber(int tableNumber)
        {
            Table? table = _tablesRepository.GetTableByNumber(tableNumber);
            return table;
        }

        //Geeft alle tafels
        public List<Table> GetAllTables()
        {
            List<Table> tables = _tablesRepository.GetAllTables();
            return tables;
        }

        //Past de tafel status aan, maar checkt of er ongeserveerde orders zijn.
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
                        break;
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

        //Checkt of de payment knop zichtbaar moet zijn.
        public bool CanTablePay(List<Order> orders)
        {
            if (orders == null || orders.Count == 0) return false;
            return orders.All(o => o.Status == OrderStatus.Served);
        }
    }
}