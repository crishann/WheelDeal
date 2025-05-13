using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models;
using Microsoft.EntityFrameworkCore; // For EF Core methods like `FirstOrDefaultAsync`
using BCrypt.Net;  // Ensure this is at the top of your file

public class AuthController : Controller
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user, string confirmPassword)
    {
        if (user.Password != confirmPassword)
        {
            ModelState.AddModelError("Password", "Passwords do not match.");
            return View(user); // Return to the registration page
        }

        // Check if email already exists using the UserService
        if (await _userService.EmailExistsAsync(user.Email))
        {
            ModelState.AddModelError("Email", "Email is already registered.");
            return View(user); // Return to the registration page
        }

        // Hash the password before storing it using BCrypt
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Correct syntax for BCrypt.Net-Next
                                                                       // This should work now

        // Save user to the database using UserService
        await _userService.AddUserAsync(user);

        return RedirectToAction("Login"); // Redirect to the Login page after successful registration
    }
}
