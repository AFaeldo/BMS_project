using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using System.Linq;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "BarangaySk")]
    public class BarangaySkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangaySkController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        // Load youth members and pass to the view so the table can render data
        public IActionResult YouthProfiles()
        {
            ViewData["Title"] = "Youth Profiling";
            var youthList = _context.YouthMembers.ToList();
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
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