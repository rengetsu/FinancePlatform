using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinancePlatform.Controllers
{
    public class ChartController : Controller
    {
        // Action method to serve the Chart.cshtml view
        public IActionResult Chart()
        {
            return View();
        }

        public IActionResult Index()
        {
            // Example data, replace with actual data from your database
            var chartData = new List<int> { 10, 20, 30, 40, 50 };

            ViewBag.ChartData = chartData;

            return View("Chart");
        }
    }
}
