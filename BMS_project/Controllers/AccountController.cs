using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BMS_project.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login page with optional role
        [HttpGet]
        public IActionResult Login(string role = null)
        {
            ViewBag.Role = role ?? "SuperAdmin"; // Default role if none provided
            return View();
        }

        // POST: Login logic
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            bool isValid = false;

            // Validate credentials based on role
            switch (role)
            {
                case "SuperAdmin":
                    isValid = username == "admin1" && password == "admin";
                    break;

                case "FederationPresident":
                    isValid = username == "federation1" && password == "federation";
                    break;

                case "BarangaySk":
                    isValid = username == "skmember1" && password == "skmem";
                    break;
            }

            if (isValid)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on role
                return role switch
                {
                    "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
                    "FederationPresident" => RedirectToAction("Dashboard", "FederationPresident"),
                    "BarangaySk" => RedirectToAction("Dashboard", "BarangaySk"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            // If login fails
            ViewBag.Error = "Invalid login credentials.";
            ViewBag.Role = role;
            return View();
        }

        // POST: Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Optional: clear session
            return RedirectToAction("Login", "Account", new { role = "SuperAdmin" }); // Default role after logout
        }

        // GET: Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
