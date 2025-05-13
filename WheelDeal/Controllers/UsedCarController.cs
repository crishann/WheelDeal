using Microsoft.AspNetCore.Mvc;

namespace WheelDeal.Controllers
{
    public class UsedCarController : Controller
    {
        public IActionResult usedcar()
        {
            return View();
        }
    }
}
