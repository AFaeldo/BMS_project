using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.ViewModels;
using BMS_project.Models;
using System.Linq;

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
            var vm = new DashboardViewModel
            {
                TotalBarangays = _context.barangays?.Count() ?? 0,
                TotalUsers = _context.Users?.Count() ?? 0
            };

            ViewData["Title"] = "Dashboard";
            return View(vm);
        }

        public IActionResult ManageUsers()
        {
            ViewData["Title"] = "Manage Users";
            return View();
        }

        public IActionResult SystemLogs()
        {
            ViewData["Title"] = "System Logs";
            return View();
        }

        public IActionResult Settings()
        {
            ViewData["Title"] = "Settings";
            return View();
        }

        // GET: show list of barangays
        public IActionResult Barangay()
        {
            ViewData["Title"] = "Manage Barangay";
            var barangays = _context.barangays?.OrderBy(b => b.Barangay_Name).ToList() ?? new List<Barangay>();
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
            var exists = _context.barangays.Any(b => b.Barangay_Name.ToLower() == BarangayName.Trim().ToLower());
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
