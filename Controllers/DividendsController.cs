using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers
{
    public class DividendsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
