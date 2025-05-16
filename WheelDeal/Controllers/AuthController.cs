using Microsoft.AspNetCore.Mvc;
using WheelDeal.Models;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

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

        await _userService.AddUserAsync(user);

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }



    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userService.GetUserByEmailAsync(username);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.First_Name);
            Console.WriteLine("Login as " + user.Email);


            return RedirectToAction("Index", "Home");
        }

        Console.WriteLine("Invalid username or password");
        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    public IActionResult Logout()
    {
        Console.WriteLine("HSession is cleare / logout");

        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }


}
