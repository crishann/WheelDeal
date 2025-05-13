using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models;

namespace WheelDeal.Controllers
{
    public class HomeController : Controller
    {
        private readonly MySqlTest _mysql;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MySqlTest mysql, ILogger<HomeController> logger)
        {
            _mysql = mysql;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
