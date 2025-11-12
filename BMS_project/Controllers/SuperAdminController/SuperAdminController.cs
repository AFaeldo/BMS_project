using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Controllers.SuperAdminController
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        public IActionResult BackupMaintenance()
        {
            ViewData["Title"] = "Backup & Maintenance";
            return View();
        }
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            return View();
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
