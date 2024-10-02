using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancePlatform.Controllers
{
    public class TableController : Controller
    {
        // Action method to serve the Table.cshtml view
        public IActionResult Table()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
