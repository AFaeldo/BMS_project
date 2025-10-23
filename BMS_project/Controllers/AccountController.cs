using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using System.Text.RegularExpressions;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Find the user in the database (table: kabataan.login)
        var user = await _context.Login
            .Include(l => l.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // Map database role text to application role keys used by [Authorize(Roles = "...")]
        string rawRole = user.Role?.Role_Name?.Trim() ?? string.Empty;
        string roleKey;

        if (rawRole.Equals("SuperAdmin", System.StringComparison.OrdinalIgnoreCase) ||
            rawRole.Equals("Super Admin", System.StringComparison.OrdinalIgnoreCase))
        {
            roleKey = "SuperAdmin";
        }
        else if (rawRole.Equals("FederationPresident", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("Federation President", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("SK Federation President", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("SK Federation", System.StringComparison.OrdinalIgnoreCase))
        {
            roleKey = "FederationPresident";
        }
        else if (rawRole.Equals("BarangaySk", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("Barangay SK", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("Barangay", System.StringComparison.OrdinalIgnoreCase) ||
                 rawRole.Equals("SK", System.StringComparison.OrdinalIgnoreCase))
        {
            roleKey = "BarangaySk";
        }
        else
        {
            // Fallback: remove whitespace and non-alphanumeric characters so "Some Role" -> "SomeRole"
            roleKey = Regex.Replace(rawRole, @"\W+", "");
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, roleKey) // normalized role key
        };

        // Create identity and principal
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Sign in
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true
            });

        // Redirect based on normalized roleKey
        if (roleKey == "SuperAdmin")
            return RedirectToAction("Dashboard", "SuperAdmin");
        else if (roleKey == "FederationPresident")
            return RedirectToAction("Dashboard", "FederationPresident");
        else if (roleKey == "BarangaySk")
            return RedirectToAction("Dashboard", "BarangaySk");

        // Default fallback
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return Content("Access Denied — You don’t have permission to view this page.");
    }
}
