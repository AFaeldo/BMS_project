using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BMS_project.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string role)
        {
            
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (username == "admin@gmail.com" && password == "1234")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

              
                return role switch
                {
                    "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
                    "FederationPresident" => RedirectToAction("Dashboard", "FederationPresident"),
                    "BarangaySk" => RedirectToAction("Dashboard", "BarangaySk"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            // If login fails, show an error message
            ViewBag.Error = "Invalid login credentials.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
