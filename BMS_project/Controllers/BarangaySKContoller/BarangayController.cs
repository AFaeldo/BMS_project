using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

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

        // GET: Create Project
        public IActionResult CreateProject()
        {
            ViewData["Title"] = "Create Project";
            return View();
        }

        // POST: Create Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = User.Identity.Name;
            var login = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (login == null || login.User == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var user = login.User;
            if (user.Barangay_ID == null)
            {
                ModelState.AddModelError("", "User is not assigned to a Barangay.");
                return View(model);
            }

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Barangay_ID == user.Barangay_ID);

            if (budget == null)
            {
                ModelState.AddModelError("", "No budget found for this Barangay.");
                return View(model);
            }

            if (model.Allocated_Amount > budget.balance)
            {
                ModelState.AddModelError("Allocated_Amount", $"Insufficient funds. Available balance: {budget.balance:C}");
                return View(model);
            }

            // Create Project
            var project = new Project
            {
                User_ID = user.User_ID,
                Project_Title = model.Project_Title,
                Project_Description = model.Project_Description,
                Date_Submitted = DateTime.Now,
                Project_Status = "Pending",
                Start_Date = model.Start_Date,
                End_Date = model.End_Date
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(); // Save to generate Project_ID

            // Create Allocation
            var allocation = new ProjectAllocation
            {
                Budget_ID = budget.Budget_ID,
                Project_ID = project.Project_ID,
                Amount_Allocated = model.Allocated_Amount
            };

            _context.ProjectAllocations.Add(allocation);
            
            // Log the action
            var log = new ProjectLog
            {
                Project_ID = project.Project_ID,
                User_ID = user.User_ID,
                Status = "Pending",
                Changed_On = DateTime.Now,
                Remarks = "Project created and submitted for approval."
            };
            _context.ProjectLogs.Add(log);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project submitted successfully!";
            return RedirectToAction(nameof(Projects));
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

        public async Task<IActionResult> Projects()
        {
            ViewData["Title"] = "Project Management";

            var username = User.Identity.Name;
            var login = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (login == null || login.User == null)
            {
                // Handle the case where user is not found (though Authorize attribute should prevent this mostly)
                return RedirectToAction("Login", "Account");
            }

            var projects = await _context.Projects
                .Where(p => p.User_ID == login.User.User_ID)
                .Include(p => p.Allocations)
                .OrderByDescending(p => p.Date_Submitted)
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> Budgets()
        {
            ViewData["Title"] = "Budget & Finance";

            var username = User.Identity.Name;
            var login = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (login == null || login.User == null || login.User.Barangay_ID == null)
            {
                // Return default empty budget if user/barangay not found
                return View(new Budget { budget = 0, disbursed = 0, balance = 0 });
            }

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Barangay_ID == login.User.Barangay_ID);

            return View(budget ?? new Budget { budget = 0, disbursed = 0, balance = 0 });
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Reports";
            return View();
        }
        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
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