using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WheelDeal.Models;

namespace WheelDeal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register UserService to be injected into constructors
            builder.Services.AddScoped<UserService>(); // Use Scoped lifetime for UserService

            // Add services to the container.
            builder.Services.AddControllersWithViews(); // Register MVC controllers and views

            // Register DbContext (AppDbContext) with MySQL connection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"), // Fetch connection string from appsettings.json
                    new MySqlServerVersion(new Version(8, 0, 36)) // Specify MySQL server version (adjust as per your setup)
                )
            );

            // Configure security and cookie policies (for production environment)
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.Secure = CookieSecurePolicy.Always; // Ensure cookies are always sent over HTTPS
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                // Restrict cookies to same-site requests
            });

            // Register antiforgery configuration to use secure cookies with SameSite attribute
            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache(); // Required

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

            app.UseAuthorization(); // Enable Authorization middleware (required for protected routes)

            // Define MVC routing pattern
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}" // Default route setup
            );

            // Start the application
            app.Run();
        }
    }
}
