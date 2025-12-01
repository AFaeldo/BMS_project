using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.ViewModels;
using BMS_project.Models;
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

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult SetNewTerm(string TermName, DateTime StartDate, DateTime EndDate)
        {
            if (string.IsNullOrWhiteSpace(TermName))
            {
                TempData["ErrorMessage"] = "Term Name is required.";
                return RedirectToAction("Dashboard");
            }

            try
            {
                // Deactivate all existing terms
                var existingTerms = _context.KabataanTermPeriods.Where(t => t.IsActive).ToList();
                foreach (var term in existingTerms)
                {
                    term.IsActive = false;
                }

                // Create new term
                var newTerm = new KabataanTermPeriod
                {
                    Term_Name = TermName,
                    Start_Date = StartDate,
                    Official_End_Date = EndDate,
                    IsActive = true
                };

                _context.KabataanTermPeriods.Add(newTerm);

                // Log
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    _context.SystemLogs.Add(new SystemLog 
                    { 
                        User_ID = userId, 
                        Remark = $"Set New Term: {TermName}", 
                        DateTime = DateTime.Now 
                    });
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "New Term Period set successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error setting new term: " + ex.Message;
            }

            return RedirectToAction("Dashboard");
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
                query = query.Where(s => s.Remark.Contains(search) || 
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
        public IActionResult AddBarangay(string BarangayName)
        {
            if (string.IsNullOrWhiteSpace(BarangayName))
            {
                TempData["ErrorMessage"] = "Barangay name is required.";
                return RedirectToAction("Barangay");
            }

            // prevent duplicates (simple check)
            var exists = _context.barangays
                .Any(b => b.Barangay_Name.ToLower() == BarangayName.Trim().ToLower());

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

            // Log
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                _context.SystemLogs.Add(new SystemLog
                {
                    User_ID = userId,
                    Remark = $"Added Barangay: {BarangayName.Trim()}",
                    DateTime = DateTime.Now
                });
            }

            _context.SaveChanges();

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