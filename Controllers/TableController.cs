using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services;
using shappo0.ViewModels;

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
            List<Table> tables = _tableService.GetAllTables();
            return View(tables);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Table(int tableNumber)
        {
            ViewBag.TableNumber = tableNumber;
            List<TableViewModel> tableOrders = _tableService.GetOrdersForTable(tableNumber);

            if (tableOrders.Count == 0)
            {
                ViewBag.Message = "There are no orders for this table.";
                return View();
            }
              
            return View(tableOrders);
        }
    }
}
