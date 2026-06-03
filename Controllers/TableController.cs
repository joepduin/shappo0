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

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        //je moet ingelogd zijn. tafels woorden doorgegeven aan de view.
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            List<TableViewModel> tablesViewModels = _tableService.GetAllTables();
            return View(tablesViewModels);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Table(int tableNumber)
        {
            TableViewModel tableViewModel = null;
            TableViewModel table = _tableService.GetTableByNumber(tableNumber);
            return View(table);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UpdateOrderLineStatus(int orderLineId, int tableNumber, int orderNumber)
        {
            _tableService.UpdateOrderLineStatus(orderLineId, OrderStatus.Served, orderNumber);
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
    }
}