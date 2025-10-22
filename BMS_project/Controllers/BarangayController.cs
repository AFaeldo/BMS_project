using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Controllers
{
    public class BarangaySkController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        public IActionResult Documents()
        {
            ViewData["Title"] = "Document Uploads";
            return View();
        }

        public IActionResult YouthProfiles()
        {
            ViewData["Title"] = "Youth Profiling";
            return View();
        }

        public IActionResult Projects()
        {
            ViewData["Title"] = "Project Management";
            return View();
        }

        public IActionResult Budgets()
        {
            ViewData["Title"] = "Budget & Finance";
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Reports";
            return View();
        }
    }
}