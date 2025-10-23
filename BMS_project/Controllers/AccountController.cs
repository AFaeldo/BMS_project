using BMS_project.Data;
using BMS_project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BMS_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewBag.Role = role ?? "SuperAdmin"; // Default role if none provided
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


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Optional: clear session
            return RedirectToAction("Login", "Account", new { role = "SuperAdmin" }); // Default role after logout
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
