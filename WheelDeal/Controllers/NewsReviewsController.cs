using Microsoft.AspNetCore.Mvc;

namespace WheelDeal.Controllers
{
    public class NewsReviewsController : Controller
    {
        public IActionResult newsreviews()
        {
            return View();
        }
    }
}
