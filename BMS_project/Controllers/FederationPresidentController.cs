using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Controllers
{
    public class FederationPresidentController : Controller
    {
        public IActionResult ComplianceMonitoring()
        {
            ViewData["Title"] = "Compliance Monitoring";
            return View();
        }
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Administrative Dashboard";
            return View();
        }

        public IActionResult ReportGeneration()
        {
            ViewData["Title"] = "Report Generation";
            return View();
        }

        public IActionResult ProjectApprovals()
        {
            ViewData["Title"] = "Project Approvals";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }
    }
}
