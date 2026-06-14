using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.ViewModels;
using shappo0.Services;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Controllers
{
    [Authorize]
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

        //De tafels overzicht pagina
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

        //Detail pagina van een tafel
        [HttpGet]
        public IActionResult Table(int tableNumber)
        {
            Table? table = _tableService.GetTableByNumber(tableNumber);

            if (table == null)
            {
                TempData["ErrorMessage"] = "Table does not exist.";
                return RedirectToAction("Index");
            }

            TableViewModel viewModel = BuildTableDetailModel(table);
            return View(viewModel);
        }

        //Bestelling serveren
        [HttpPost]
        public ActionResult UpdateOrderLineStatus(OrderLine orderLine, int tableNumber)
        {
            _orderService.UpdateOrderLineStatus(orderLine, OrderStatus.Served);

            return RedirectToAction("Table", new { tableNumber = tableNumber });
        }

        //Veranderd de tafel status, TRUE = bezet, FALSE = vrij
        [HttpPost]
        public ActionResult UpdateTableStatus(Table table)
        {
            if (!_tableService.UpdateTableStatus(table))
            {
                TempData["ErrorMessage"] = "Status cannot be changed, still has unserved orders.";
                return RedirectToAction("Table", new { tableNumber = table.TableNumber });
            }

            TempData["ConfirmMessage"] = "Table has been changed successfully.";

            if (table.Status)
            {
                return RedirectToAction("Table", new { tableNumber = table.TableNumber });
            }

            return RedirectToAction("Index", "Table");
        }

        //Maakt het view model voor 1 tafel in het overzicht
        private TableViewModel BuildTableOverviewModel(Table table)
        {
            Visit? visit = _visitService.GetActiveVisitForTable(table.TableNumber);
            if (visit == null) return new() { Table = table };

            OrderStatus drinkStatus = _orderService.GetOrderStatusByVisit(visit, ProductType.Drink);
            OrderStatus foodStatus = _orderService.GetOrderStatusByVisit(visit, ProductType.Main); //Alles anders dan drink is food

            return new(table, drinkStatus, foodStatus);
        }

        //Maakt het viewmodel voor een tafel detail pagina
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

            bool canPay = _tableService.CanTablePay(orders);
            return new(table, orders, canPay);
        }
    }
}