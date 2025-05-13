using Microsoft.AspNetCore.Mvc;

namespace WheelDeal.Controllers
{
    public class NewCarController : Controller
    {
        public IActionResult newcar()
        {
            return View();
        }
    }
}
