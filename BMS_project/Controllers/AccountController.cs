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
    private readonly IEmailService _emailService;

    public AccountController(ApplicationDbContext context, ISystemLogService systemLogService, IEmailService emailService)
    {
        _context = context;
        _systemLogService = systemLogService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        // Fetch latest active announcement
        var announcement = await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.Date_Created)
            .FirstOrDefaultAsync();

        if (announcement != null)
        {
            ViewBag.Announcement = announcement;
        }

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

        // Check Email Verification
        if (user.User != null && !user.User.IsEmailVerified)
        {
            ViewBag.Error = "Please verify your email address before logging in.";
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
            // 1. Get System's Global Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);

            if (activeTerm != null)
            {
                // 2. Check if user has a Service Record for THIS active term
                var userActiveRecord = user.User.ServiceRecords?
                    .FirstOrDefault(r => r.Term_ID == activeTerm.Term_ID && r.Status == "Active");

                if (userActiveRecord == null)
                {
                    // No record for the current term -> Archive User and Deny Login
                    
                    // Archive User
                    user.User.IsArchived = true;
                    _context.Users.Update(user.User);

                    // Mark old active records as "Term Ended"
                    var oldRecords = user.User.ServiceRecords?.Where(r => r.Status == "Active").ToList();
                    if (oldRecords != null && oldRecords.Any())
                    {
                        foreach (var oldRecord in oldRecords)
                        {
                            oldRecord.Status = "Term Ended";
                            oldRecord.Actual_End_Date = DateTime.Now;
                            _context.KabataanServiceRecords.Update(oldRecord);
                        }
                    }

                    await _context.SaveChangesAsync();

                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }
                else
                {
                    // User has an active record for the current term.
                    // Ensure they are NOT archived (in case they were re-elected but IsArchived wasn't cleared)
                    if (user.User.IsArchived)
                    {
                        user.User.IsArchived = false;
                        _context.Users.Update(user.User);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                // No active term defined in system.
                // Decision: Deny access to prevent operations in undefined state? Or allow limited access?
                // Safe bet: Deny with message.
                ViewBag.Error = "No active term is currently set in the system. Please contact SuperAdmin.";
                return View();
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
    public async Task<IActionResult> VerifyEmail(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        if (user == null)
        {
            return NotFound("Invalid verification token.");
        }

        user.IsEmailVerified = true;
        user.VerificationToken = null; // Clear token after use

        await _context.SaveChangesAsync();

        ViewBag.Success = "Email verified successfully! You can now login.";
        return View("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return Content("Access Denied — You don’t have permission to view this page.");
    }

    [HttpGet]
    public IActionResult ResendVerification()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResendVerification(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.Message = "Please enter your email address.";
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            // Don't reveal if user exists or not for security, or do? 
            // Existing code reveals username existence in ForgotPassword, so we might as well be consistent or better.
            // But standard practice is "If an account exists, an email has been sent."
            // However, for verification, if they are already verified, we should tell them "Account already verified."
            ViewBag.Message = "If an account with that email exists and is unverified, a verification link has been sent.";
            return View();
        }

        if (user.IsEmailVerified)
        {
            ViewBag.Message = "This account is already verified. Please login.";
            return View();
        }

        // Generate new token
        var token = Guid.NewGuid().ToString();
        user.VerificationToken = token;
        _context.Update(user);
        await _context.SaveChangesAsync();

        // Send Email
        var verifyUrl = Url.Action("VerifyEmail", "Account", new { token = token }, Request.Scheme);
        var message = $"Please verify your email by clicking <a href='{verifyUrl}'>here</a>.";
        
        try
        {
            await _emailService.SendEmailAsync(user.Email, "Resend Email Verification", message);
            ViewBag.Message = "Verification link sent! Please check your email.";
            
            // Log the resend verification event
            if (user.User_ID > 0) // Assuming User_ID is populated
            {
                await _systemLogService.LogAsync(user.User_ID, "Resend Verification", $"Verification email resent to {user.Email}");
            }
        }
        catch (Exception ex)
        {
            // Log error?
            ViewBag.Message = "Error sending email. Please try again later. " + ex.Message;
        }

        return View();
    }

}