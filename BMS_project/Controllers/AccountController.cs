using BMS_project.Data;
using BMS_project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Please enter username and password.";
            return View();
        }

        username = username.Trim();
        password = password.Trim();

        // Case-insensitive lookup (translate to lower for EF)
        var lowerUsername = username.ToLower();
        var user = await _context.Login
            .Include(l => l.Role)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == lowerUsername);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        var passwordHasher = new PasswordHasher<Login>();
        PasswordVerificationResult verification = PasswordVerificationResult.Failed;

        var storedPassword = (user.Password ?? string.Empty).Trim();

        if (!string.IsNullOrEmpty(storedPassword))
        {
            verification = passwordHasher.VerifyHashedPassword(user, storedPassword, password);
        }

        if (verification == PasswordVerificationResult.Failed)
        {
            if (storedPassword != password)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            user.Password = passwordHasher.HashPassword(user, password);
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Verification succeeded (either Success or SuccessRehashNeeded)
            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Re-hash with current algorithm & save to DB
                user.Password = passwordHasher.HashPassword(user, password);
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
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
            roleKey = System.Text.RegularExpressions.Regex.Replace(rawRole, @"\W+", "");
        }

        // Create claims & sign-in
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, roleKey)
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = true });

        // Redirect based on role
        return roleKey switch
        {
            "SuperAdmin" => RedirectToAction("Dashboard", "SuperAdmin"),
            "FederationPresident" => RedirectToAction("Dashboard", "FederationPresident"),
            "BarangaySk" => RedirectToAction("Dashboard", "BarangaySk"),
            _ => RedirectToAction("Index", "Home"),
        };
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


    // FORGOT PASSWORD 

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            ViewBag.Message = "Please enter your username.";
            return View();
        }

        var user = await _context.Login.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            ViewBag.Message = "No account found with this username.";
            return View();
        }

        ViewBag.Message = "Username found. Please contact your administrator to reset your password.";
        return View();
    }

}