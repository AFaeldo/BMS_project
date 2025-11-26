using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BMS_project.Controllers
{
    [Authorize(Roles = "FederationPresident")]
    public class FederationPresidentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FederationPresidentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var vm = new DashboardViewModel
            {
                TotalBarangays = _context.barangays?.Count() ?? 0,
                TotalUsers = _context.Users?.Count() ?? 0
            };
            return View(vm);
        }

        public IActionResult ComplianceMonitoring()
        {
            ViewData["Title"] = "Compliance Monitoring";
            return View();
        }

        public IActionResult ReportGeneration()
        {
            ViewData["Title"] = "Report Generation";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }

        //  Updated: Project Approvals now pulls data from MySQL
        public IActionResult ProjectApprovals()
        {
            ViewData["Title"] = "Project Approvals";
            return View();
        }

        // Handles Approve / Reject actions
        [HttpPost]
        public IActionResult UpdateStatus(int projectId, string status, string remarks)
        {
            return View();
        }

        public IActionResult Budget()
        {
            ViewData["Title"] = "Barangay Budgets";
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveProfile()
        {
            TempData["SuccessMessage"] = "Profile saved successfully!";
            return RedirectToAction("Profile");
        }

        public IActionResult UserManagement()
        {
            ViewData["Title"] = "User Management";
            return View();
        }
    }
}
