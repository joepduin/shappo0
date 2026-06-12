using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.ViewModels;
using shappo0.Services;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Controllers
{
    public class TableController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IOrderService _orderService;
        private readonly IVisitService _visitService;

        public TableController(ITableService tableService, IOrderService orderService, IVisitService visitService)
        {
            _tableService = tableService;
            _orderService = orderService;
            _visitService = visitService;
        }

        //je moet ingelogd zijn. tafels woorden doorgegeven aan de view.
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            List<Table> tables= _tableService.GetAllTables();
            List<TableViewModel> tableViewModels = new List<TableViewModel>();

            foreach (Table table in tables)
            {
                tableViewModels.Add(BuildTableOverviewModel(table));
            }

            return View(tableViewModels);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Table(int tableNumber)
        {
            Table table = _tableService.GetTableByNumber(tableNumber);

            if (table == null)
            {
                TempData["ErrorMessage"] = "Tafel bestaat niet.";
                return RedirectToAction("Index");
            }

            TableViewModel viewModel = BuildTableDetailModel(table);
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateOrderLineStatus(OrderLine orderLine, int tableNumber)
        {
            _orderService.UpdateOrderLineStatus(orderLine, OrderStatus.Served);


            return RedirectToAction("Table", "Table", new { tableNumber = tableNumber });
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateTableStatus(Table table)
        {
            if (_tableService.UpdateTableStatus(table))
            {
                TempData["ConfirmMessage"] = "Tafel is succesvol gewijzigd.";
                if (table.Status == true)
                {
                    return RedirectToAction("Table", "Table", new { tableNumber = table.TableNumber });
                }
                return RedirectToAction("Index", "Table");
            }
            

                TempData["ErrorMessage"] = "Status kan niet veranderd worden, heeft nog ongeserveerde orders.";
            return RedirectToAction("Table", "Table", new { tableNumber = table.TableNumber });
        }

        private TableViewModel BuildTableOverviewModel(Table table)
        {
            List<OrderLine> orderLines = _orderService.GetActiveOrderLinesByTableNumber(table.TableNumber);

            OrderStatus drinkStatus = orderLines
                .Where(ol => ol.Product.ProductType == ProductType.Drink)
                .Select(ol => ol.Status)
                .DefaultIfEmpty(OrderStatus.Served)
                .Max();

            OrderStatus foodStatus = orderLines
                .Where(ol => ol.Product.ProductType != ProductType.Drink)
                .Select(ol => ol.Status)
                .DefaultIfEmpty(OrderStatus.Served)
                .Max();

            return new(table, drinkStatus, foodStatus);
        }

        private TableViewModel BuildTableDetailModel(Table table)
        {
            Visit? visit = _visitService.GetActiveVisitForTable(table.TableNumber);
            List<Order> orders = new List<Order>();
            if (visit != null)
            {
                orders = _orderService.GetOrdersByVisitId(visit.VisitId);
                foreach (Order order in orders)
                {
                    order.OrderLines = _orderService.GetOrderLinesByOrderNumber(order.OrderNumber);
                }
            }
            return new(table, orders);
        }
    }
}