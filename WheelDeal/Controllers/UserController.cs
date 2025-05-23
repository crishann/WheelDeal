using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models; // Make sure to include your models namespace (Car, User etc.)
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment
using System.IO;
using System.Threading.Tasks;
using System.Linq; // For .FirstOrDefault()
using System.Security.Claims; // Needed for HttpContext.User.FindFirstValue to get UserId
using WheelDeal.Services; // <--- ADD THIS LINE! This is the fix for CS0246

namespace WheelDeal.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserService _userService; // This line (15 in your error)

        // Constructor: ASP.NET Core's Dependency Injection will automatically provide these
        public UserController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserService userService) // This line (17 in your error)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userService = userService;
        }

        // --- HTTP GET Action to display the "Sell Car" form ---
        public IActionResult SellCar()
        {
            var car = new Car();
            car.Location = "Cebu City, Central Visayas, Philippines";

            return View(car);
        }

        // --- HTTP POST Action to handle the "Sell Car" form submission ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellCar(Car car, IFormFile[] uploadedImages)
        {
            car.CreatedAt = DateTime.Now;
            car.Status = "available";

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                car.UserId = currentUserId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to list a car. Please log in.");
                return View(car);
            }

            if (car.Condition == "New")
            {
                car.Mileage = 0;
                ModelState.Remove(nameof(car.Mileage));
            }
            else if (car.Condition == "Used" && car.Mileage == null)
            {
                ModelState.AddModelError(nameof(car.Mileage), "Mileage is required for used cars.");
            }

            if (uploadedImages == null || uploadedImages.Length == 0)
            {
                ModelState.AddModelError("uploadedImages", "Please upload at least one image for your car.");
            }

            if (!ModelState.IsValid)
            {
                return View(car);
            }

            string primaryImagePath = null;
            if (uploadedImages != null && uploadedImages.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var firstFile = uploadedImages.FirstOrDefault();
                if (firstFile != null && firstFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(firstFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await firstFile.CopyToAsync(fileStream);
                    }
                    primaryImagePath = $"/uploads/cars/{uniqueFileName}";
                }
                car.ImagePath = primaryImagePath;
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your car has been listed successfully!";
            return RedirectToAction("Index", "Home");
        }


        // --- New: HTTP GET Action for User Profile ---
        public async Task<IActionResult> UserProfile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your profile.";
                return RedirectToAction("Login", "Auth"); // Assuming your login is in AuthController
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                TempData["ErrorMessage"] = "Could not retrieve your user ID. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User profile not found.";
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // --- New: HTTP POST Action for User Profile Updates ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(User user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to update your profile.";
                return RedirectToAction("Login", "Auth");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                TempData["ErrorMessage"] = "Could not verify your user ID. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            if (user.Id != currentUserId)
            {
                TempData["ErrorMessage"] = "Unauthorized attempt to update profile.";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.Remove(nameof(user.Password));
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                var existingUser = await _userService.GetUserByIdAsync(user.Id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "User profile not found for update.";
                    return RedirectToAction("Index", "Home");
                }

                existingUser.First_Name = user.First_Name;
                existingUser.Last_Name = user.Last_Name;
                existingUser.Email = user.Email;
                existingUser.Contact = user.Contact;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    // Remember to HASH the password here in a real application!
                    existingUser.Password = user.Password; // DANGER: STORE HASHED PASSWORD ONLY!
                }

                await _userService.UpdateUserAsync(existingUser);

                TempData["Message"] = "Your profile has been updated successfully!";
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again.");
                return View(user);
            }
        }
    }
}