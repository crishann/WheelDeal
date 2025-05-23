using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WheelDeal.Services; // <--- ADD THIS LINE to resolve UserService not found

public class AuthController : Controller
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
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
            return View(user);
        }

        if (await _userService.EmailExistsAsync(user.Email))
        {
            ModelState.AddModelError("Email", "Email is already registered.");
            return View(user);
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        // Console.WriteLine("Invalid username or password"); // This line doesn't make sense here, remove or change it
        await _userService.AddUserAsync(user);

        TempData["Message"] = "Registration successful! Please log in."; // Add a success message
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(string email, string password) // Changed username to email for consistency
    {
        var user = await _userService.GetUserByEmailAsync(email); // Use GetUserByEmailAsync

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            // Create claims including the user role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Store user ID
                new Claim(ClaimTypes.Name, user.Email),                  // Store email as principal name
                new Claim(ClaimTypes.GivenName, user.First_Name),        // Store first name
                new Claim(ClaimTypes.Role, user.Role)                    // Critical for Authorize with roles
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            Console.WriteLine($"Login as {user.Email} with role {user.Role}"); // Use string interpolation

            return RedirectToAction("Index", "Home");
        }

        Console.WriteLine("Invalid email or password"); // Changed username to email
        ViewBag.Error = "Invalid email or password.";   // Changed username to email
        return View();
    }

    [Authorize] // Only authenticated users can logout
    public async Task<IActionResult> Logout() // Made async because HttpContext.SignOutAsync is async
    {
        // Sign out the user
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // HttpContext.Session.Clear(); // Only if you are actively using session for user authentication data
        // (less common with cookie authentication for identity)

        Console.WriteLine("User logged out.");

        return RedirectToAction("Login");
    }
}