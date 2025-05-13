using Microsoft.AspNetCore.Mvc;

namespace WheelDeal.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult login()
        {
            return View();
        }
        public IActionResult register()
        {
            return View();
        }
    }
}
