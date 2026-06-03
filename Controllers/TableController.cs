using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.ViewModels;
using shappo0.Services;

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
            TableViewModel table = _tableService.GetTableByNumber(tableNumber);
            return View(table);
        }
    }
}
