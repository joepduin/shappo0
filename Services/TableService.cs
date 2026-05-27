using AspNetCoreGeneratedDocument;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.ViewModels;
using shappo0.Extensions;
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

        public List<TableStatusViewModel> GetAllTableStatus()
        {
            List<Table> tables = _tablesRepository.GetAllTables();
            List<Visit> visits = _visitRepository.GetActiveVisits();

            foreach (Visit visit in visits)
            {
                
                visit.Orders = _orderRepository.GetOrdersByVisitId(visit.VisitId);
                foreach (Order order in visit.Orders)
                {
                    order.OrderLines = _orderLineRepository.GetOrderLinesByOrderNumber(order.OrderNumber);
                }
            }

            return ToViewModel(visits, tables);
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

        private List<TableStatusViewModel> ToViewModel(List<Visit> visits, List<Table> tables)
        {
            List<TableStatusViewModel> viewModels = new List<TableStatusViewModel>();

            foreach(Table table in tables)
            {
                viewModels.Add(new(table.TableNumber, table.Status));
            }

            foreach (Visit visit in visits)
            {
                TableOrderStatus DrinkStatus = TableOrderStatus.Unknown;
                TableOrderStatus FoodStatus = TableOrderStatus.Unknown;

                foreach (Order order in visit.Orders)
                {
                    foreach(OrderLine orderLine in order.OrderLines)
                    {
                        if (orderLine.Product.ProductType == ProductType.Drink)
                        {
                            DrinkStatus = ToOrderStatus(orderLine.Status, DrinkStatus);
                        }
                        else
                        {
                            FoodStatus = ToOrderStatus(orderLine.Status, FoodStatus);
                        }
                    }
                }

                //edit viewmodel list, geen idee hoe je een list moet editen
            }

            return viewModels;
        }

        private TableOrderStatus ToOrderStatus(OrderStatus orderStatus, TableOrderStatus currentStatus)
        {
            if (currentStatus == TableOrderStatus.Unknown)
            {
                return orderStatus.ToTableStatus();
            }

            if (currentStatus == TableOrderStatus.Ordered)
            {
                if (orderStatus == OrderStatus.Running) return TableOrderStatus.Ordered;
                if (orderStatus == OrderStatus.Preparing) return TableOrderStatus.BeingPrepared;
                if (orderStatus == OrderStatus.Completed) return TableOrderStatus.ReadyToBeServed;
            }

            if (currentStatus == TableOrderStatus.BeingPrepared)
            {
                if (orderStatus == OrderStatus.Running) return TableOrderStatus.BeingPrepared;
                if (orderStatus == OrderStatus.Preparing) return TableOrderStatus.BeingPrepared;
                if (orderStatus == OrderStatus.Completed) return TableOrderStatus.ReadyToBeServed;
            }

            if (currentStatus == TableOrderStatus.ReadyToBeServed)
            {
                return TableOrderStatus.ReadyToBeServed;
            }

            return currentStatus; //fallback bij error
        }
    }
}