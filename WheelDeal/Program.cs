using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WheelDeal.Models;
using WheelDeal.Services; // <--- ADD THIS LINE to resolve UserService not found

namespace WheelDeal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register UserService to be injected into constructors
            builder.Services.AddScoped<UserService>(); // This line (15) needs UserService to be found

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register DbContext (AppDbContext) with MySQL connection
            builder.Services.AddDbContext<AppDbContext>(options => // AppDbContext also needs a using directive if not in this namespace
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36)) // Specify MySQL server version (adjust as per your setup)
                )
            );

            // Configure security and cookie policies (for production environment)
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.Secure = CookieSecurePolicy.Always; // Ensure cookies are always sent over HTTPS
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });

            builder.Services.AddAuthorization();


            // Register antiforgery configuration to use secure cookies with SameSite attribute
            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache(); // Required

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // Production mode configurations
                app.UseExceptionHandler("/Home/Error"); // Global error handling
                app.UseHsts(); // Enforce HTTP Strict Transport Security (for production)
            }
            else
            {
                // Development environment configurations
                app.UseDeveloperExceptionPage(); // Show detailed errors during development
            }


            app.UseSession();

            // Set up middleware pipeline
            app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
            app.UseStaticFiles(); // Serve static files (CSS, JS, images, etc.)

            app.UseRouting(); // Enable routing
            app.UseAuthentication();
            app.UseAuthorization(); // Enable Authorization middleware (required for protected routes)

            // Define MVC routing pattern
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}" // Default route setup
            );

            // Start the application
            app.Run();
        }
    }
}