using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.ViewModels;
using BMS_project.Models;
using BMS_project.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace BMS_project.Controllers.SuperAdminController
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;
        private readonly ITermService _termService;

        public SuperAdminController(ApplicationDbContext context, ISystemLogService systemLogService, ITermService termService)
        {
            _context = context;
            _systemLogService = systemLogService;
            _termService = termService;
        }

        public IActionResult BackupMaintenance()
        {
            ViewData["Title"] = "Backup & Maintenance";
            return View();
        }

        public IActionResult Dashboard()
        {
            var activeTerm = _context.KabataanTermPeriods.FirstOrDefault(t => t.IsActive);
            var vm = new DashboardViewModel
            {
                TotalBarangays = _context.barangays?.Count() ?? 0,
                TotalUsers = _context.Users?.Count() ?? 0,
                CurrentTerm = activeTerm?.Term_Name ?? "No Active Term"
            };

            ViewData["Title"] = "Dashboard";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewTerm(string TermName, DateTime StartDate, DateTime EndDate)
        {
            if (string.IsNullOrWhiteSpace(TermName))
            {
                TempData["ErrorMessage"] = "Term Name is required.";
                return Redirect("/SuperAdmin/Dashboard");
            }

            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.TryParse(userIdStr, out int parsedId) ? parsedId : 0;

                var result = await _termService.CreateTermAsync(TermName, StartDate, EndDate, userId);

                if (result.IsSuccess)
                {
                    TempData["DashboardSuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["DashboardErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["DashboardErrorMessage"] = "Error setting new term: " + ex.Message;
            }

            return Redirect("/SuperAdmin/Dashboard");
        }

        public IActionResult ManageUsers()
        {
            ViewData["Title"] = "Manage Users";
            return View();
        }

        public async Task<IActionResult> SystemLogs(int page = 1, string search = "")
        {
            ViewData["Title"] = "System Logs";
            const int pageSize = 10;

            // Include User and Login to display Username/Name
            var query = _context.SystemLogs
                .Include(s => s.User)
                .ThenInclude(u => u.Login)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => (s.Remark != null && s.Remark.Contains(search)) || 
                                         (s.User != null && (s.User.First_Name.Contains(search) || s.User.Last_Name.Contains(search))) ||
                                         (s.User != null && s.User.Login != null && s.User.Login.Username.Contains(search)));
            }

            int totalItems = await query.CountAsync();
            
            var logs = await query.OrderByDescending(s => s.DateTime)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Search = search;

            return View(logs);
        }

        public IActionResult Settings()
        {
            ViewData["Title"] = "Settings";
            return View();
        }

        // GET: show list of barangays WITH PAGINATION (10 per page) + search
        [HttpGet]
        public IActionResult Barangay(int page = 1, string search = "")
        {
            const int pageSize = 10;

            ViewData["Title"] = "Manage Barangay";

            var query = _context.barangays.AsQueryable();

            // search filter (optional)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b => b.Barangay_Name.Contains(search));
            }

            // total items for pagination
            int totalItems = query.Count();

            // only records for THIS page (10 max)
            var barangays = query
                .OrderBy(b => b.Barangay_Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // pass data to the view
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Search = search;

            return View(barangays);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBarangay(string BarangayName)
        {
            if (string.IsNullOrWhiteSpace(BarangayName))
            {
                TempData["ErrorMessage"] = "Barangay name is required.";
                return RedirectToAction("Barangay");
            }

            // prevent duplicates (simple check)
            var exists = await _context.barangays
                .AnyAsync(b => b.Barangay_Name.ToLower() == BarangayName.Trim().ToLower());

            if (exists)
            {
                TempData["ErrorMessage"] = "A barangay with that name already exists.";
                return RedirectToAction("Barangay");
            }

            var barangay = new Barangay
            {
                Barangay_Name = BarangayName.Trim()
            };

            _context.barangays.Add(barangay);
            await _context.SaveChangesAsync();

            // Log
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                await _systemLogService.LogAsync(userId, "Add Barangay", $"Added Barangay: {BarangayName.Trim()}", "Barangay", barangay.Barangay_ID);
            }

            TempData["SuccessMessage"] = "Barangay added successfully.";
            return RedirectToAction("Barangay");
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveProfile(string Barangay, string PostalAddress, string Zone, string District, string City)
        {
            TempData["SuccessMessage"] = "Profile saved successfully!";
            return RedirectToAction("Profile");
        }
    }
}