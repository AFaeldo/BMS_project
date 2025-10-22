using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return role switch
                {
                    "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
                    "FederationPresident" => RedirectToAction("Dashboard", "FederationPresident"),
                    "BarangaySk" => RedirectToAction("Dashboard", "BarangaySk"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ViewBag.Error = "Invalid login credentials.";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }

        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Forgot Password";
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }
    }
}
