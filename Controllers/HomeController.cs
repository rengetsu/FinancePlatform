using FinancePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinancePlatform.Controllers
{
    public class HomeController : Controller
    {
        // POST: ProcessText
        [HttpPost]
        public ActionResult ProcessText(TextModel model)
        {
            // Create an instance of the TextProcessor class
            TextProcessor processor = new TextProcessor();

            // Process the text from the model
            model.ProcessedText = processor.ProcessText(model.InputText);

            // Pass the model back to the view with the processed text
            return View("Index", model);
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new TextModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
