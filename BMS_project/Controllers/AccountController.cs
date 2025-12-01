using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Services;
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
    private readonly ISystemLogService _systemLogService;

    public AccountController(ApplicationDbContext context, ISystemLogService systemLogService)
    {
        _context = context;
        _systemLogService = systemLogService;
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
            .Include(l => l.User)
                .ThenInclude(u => u.Barangay)
            .Include(l => l.User)
                .ThenInclude(u => u.ServiceRecords)
                    .ThenInclude(sr => sr.KabataanTermPeriod)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == lowerUsername);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // Check if Archived
        if (user.User != null && user.User.IsArchived)
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

        // Term Expiry Check (Skip for SuperAdmin)
        // Assuming SuperAdmin role ID is 1 or name is "SuperAdmin"
        bool isSuperAdmin = (user.Role_ID == 1) || 
                            (user.Role != null && (user.Role.Role_Name == "SuperAdmin" || user.Role.Role_Name == "Super Admin"));

        if (!isSuperAdmin && user.User != null)
        {
            var activeRecord = user.User.ServiceRecords?.FirstOrDefault(r => r.Status == "Active");
            
            // If they have an active record, check date
            if (activeRecord != null && activeRecord.KabataanTermPeriod != null)
            {
                if (activeRecord.KabataanTermPeriod.Official_End_Date < DateTime.Today)
                {
                    // Term Ended -> Auto Archive
                    user.User.IsArchived = true;
                    activeRecord.Status = "Term Ended";
                    
                    _context.Users.Update(user.User);
                    _context.KabataanServiceRecords.Update(activeRecord);
                    await _context.SaveChangesAsync();

                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }
            }
            else if (!isSuperAdmin)
            {
                // Non-admin users MUST have an active term record to login? 
                // Or if they are just created but no term assigned (unlikely with new logic), let them in?
                // Requirement: "When a new user is created... assign them the Current Active Term ID."
                // So if they exist and are not archived, they should have one. 
                // If missing, it might be legacy data. We'll allow login but maybe warn? 
                // For strictness, let's allow but log or ignore. 
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
            new Claim(ClaimTypes.Role, roleKey),
            new Claim(ClaimTypes.NameIdentifier, (user.User_ID ?? 0).ToString()) // Handle nullable int
        };

        // Add Barangay_ID claim if it exists
        if (user.User?.Barangay_ID != null)
        {
            claims.Add(new Claim("Barangay_ID", user.User.Barangay_ID.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = true });

        // Store Barangay Name in Session
        if (user.User?.Barangay != null)
        {
            HttpContext.Session.SetString("BarangayName", user.User.Barangay.Barangay_Name);
        }

        // LOGGING
        if (user.User_ID.HasValue)
        {
            try 
            {
                await _systemLogService.LogAsync(user.User_ID.Value, "Login", "User Logged In");
            }
            catch { /* Ignore logging errors */ }
        }


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
        // Log Logout
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId) && userId > 0)
            {
                await _systemLogService.LogAsync(userId, "Logout", "User Logged Out");
            }
        }
        catch { }

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