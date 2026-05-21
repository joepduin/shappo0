using AspNetCoreGeneratedDocument;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.ViewModels;
using System.Collections.Generic;

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

        public List<Table> GetAllTables()
        {
            return _tablesRepository.GetAllTables();
        }

        public List<TableViewModel> GetOrdersForTable(int tableNumber)
        {
            List<TableViewModel> tableViewModels = new List<TableViewModel>();

            Visit visit = _visitRepository.GetActiveVisitForTable(tableNumber);
            if (visit == null) return tableViewModels;

            visit.Orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
            if (visit.Orders == null) return tableViewModels;

            foreach (Order order in visit.Orders)
            {
                order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);
                tableViewModels.Add(new TableViewModel(order.OrderNumber, order.Status, order.OrderLines));
            }

            return tableViewModels;
        }
    }
}
