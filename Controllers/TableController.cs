using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Controllers
{
    public class TableController : Controller
    {
        private readonly ITablesRepository _tablesRepository;

        public TableController(ITablesRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        //je moet ingelogd zijn. tafels woorden doorgegeven aan de view.
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            List<Table> tables = _tablesRepository.GetAllTables();
            return View(tables);
        }
    }
}
