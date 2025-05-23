using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models;
using WheelDeal.Services;

namespace WheelDeal.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(UserService userService, ILogger<HomeController> logger) // Renamed parameter
        {
            _userService = userService;
            _logger = logger;
        }
        [Authorize(Roles = "user,admin")]
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
