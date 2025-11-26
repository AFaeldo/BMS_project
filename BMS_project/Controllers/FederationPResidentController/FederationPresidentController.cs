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

        public IActionResult ProjectApprovals()
        {
            ViewData["Title"] = "Project Approvals";
            return View();
        }

        // Load list of budgets and pass to view
        public IActionResult Budget()
        {
            ViewData["Title"] = "Barangay Budgets";

            var budgets = _context.Budgets
                .Include(b => b.Barangay)
                .OrderBy(b => b.Barangay.Barangay_Name)
                .ToList();

            // barangays for the dropdown
            ViewBag.Barangays = new SelectList(
                _context.barangays.OrderBy(b => b.Barangay_Name).ToList(),
                "Barangay_ID",
                "Barangay_Name"
            );

            return View(budgets);
        }

        // Accept form post and create or update a budget record (add allotment to existing barangay budget)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBarangayBudget(int BarangayId, decimal Allotment)
        {
            if (Allotment <= 0)
            {
                TempData["ErrorMessage"] = "Allotment must be greater than zero.";
                return RedirectToAction("Budget");
            }

            var barangay = _context.barangays.FirstOrDefault(b => b.Barangay_ID == BarangayId);
            if (barangay == null)
            {
                TempData["ErrorMessage"] = "Selected barangay not found.";
                return RedirectToAction("Budget");
            }

            // Find existing budget for the barangay
            // If you expect multiple budget rows per barangay (history), adjust logic accordingly.
            var existingBudget = _context.Budgets.FirstOrDefault(b => b.Barangay_ID == BarangayId);

            if (existingBudget != null)
            {
                // Add the new allotment to the total allotment and increase balance by the same amount.
                existingBudget.budget += Allotment;
                existingBudget.balance += Allotment;

                _context.Budgets.Update(existingBudget);
            }
            else
            {
                // Create new budget record — disbursed starts at 0, balance = allotment
                var budget = new Budget
                {
                    Barangay_ID = barangay.Barangay_ID,
                    budget = Allotment,
                    disbursed = 0m,
                    balance = Allotment
                };

                _context.Budgets.Add(budget);
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Budget updated successfully.";
            return RedirectToAction("Budget");
        }

        // Call this when a project is approved to allocate amount to a project.
        // This action reduces the barangay balance and increases disbursed.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllocateToProject(int BarangayId, int ProjectId, decimal AllocatedAmount)
        {
            if (AllocatedAmount <= 0)
            {
                return BadRequest("Allocated amount must be greater than zero.");
            }

            var budget = _context.Budgets.FirstOrDefault(b => b.Barangay_ID == BarangayId);
            if (budget == null)
            {
                return BadRequest("No budget found for the selected barangay.");
            }

            if (budget.balance < AllocatedAmount)
            {
                return BadRequest("Insufficient barangay balance for this allocation.");
            }

            budget.disbursed += AllocatedAmount;
            budget.balance -= AllocatedAmount;

            _context.Budgets.Update(budget);
            _context.SaveChanges();

            return Ok(new { success = true, remaining = budget.balance });
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
    }
}
