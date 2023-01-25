using HtmlGenforStorybook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HtmlGenforStorybook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var header = new HeaderViewModel
            {
                Title = "Real World Title",
                Description = "Real World Description"
            };

            var footer = new FooterViewModel
            {
                Title = "Another Real World Title",
                Description = "Another Real World Description"
            };

            return View((header, footer));
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