using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.ViewModels;

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
        public IActionResult Barangay()
        {
            ViewData["Title"] = "Manage Barangay";
            return View();
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
