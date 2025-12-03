using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BMS_project.Controllers
{
    public class FundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public FundsController(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        // PART A: Super Admin - Set Total Federation Budget
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetTotalBudget(decimal amount)
        {
            if (amount <= 0)
            {
                TempData["DashboardErrorMessage"] = "Amount must be greater than zero.";
                return RedirectToAction("Dashboard", "SuperAdmin");
            }

            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null)
            {
                TempData["DashboardErrorMessage"] = "No Active Term found. Please set a term first.";
                return RedirectToAction("Dashboard", "SuperAdmin");
            }

            var existingFund = await _context.FederationFunds.FirstOrDefaultAsync(f => f.Term_ID == activeTerm.Term_ID);
            
            if (existingFund != null)
            {
                // Add to the existing fund instead of replacing
                existingFund.Total_Amount += amount;
                _context.FederationFunds.Update(existingFund);
            }
            else
            {
                var newFund = new FederationFund
                {
                    Term_ID = activeTerm.Term_ID,
                    Total_Amount = amount,
                    Allocated_To_Barangays = 0
                };
                _context.FederationFunds.Add(newFund);
            }

            await _context.SaveChangesAsync();
            
            // Log
            // int userId = GetUserId(); // Helper to get ID
            // await _systemLogService.LogAsync(userId, "Set Fund", $"Set Federation Fund: {amount}", "FederationFund", 0);

            TempData["SuccessMessage"] = "Federation fund added successfully.";
            return RedirectToAction("Dashboard", "SuperAdmin");
        }

        // PART B: Federation President - Distribute to Barangay
        [Authorize(Roles = "FederationPresident")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToBarangay(int barangayId, decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be greater than zero.");

            // 1. Get Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null) return BadRequest("No Active Term found.");

            // 2. Get Federation Fund for this term
            var fedFund = await _context.FederationFunds
                .FirstOrDefaultAsync(f => f.Term_ID == activeTerm.Term_ID);

            if (fedFund == null) return BadRequest("Federation Fund has not been set for this term.");

            // 3. VALIDATION: Check if we have enough money left to distribute
            // Available = Total - AlreadyDistributed
            if (fedFund.Allocated_To_Barangays + amount > fedFund.Total_Amount)
            {
                var remaining = fedFund.Total_Amount - fedFund.Allocated_To_Barangays;
                return BadRequest($"Federation funds insufficient. Only {remaining:C} is available for distribution.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 4. Update Federation Fund Tracker
                    fedFund.Allocated_To_Barangays += amount;
                    _context.FederationFunds.Update(fedFund);

                    // 5. Add/Update Barangay Budget (Table: budget)
                    var barangayBudget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Barangay_ID == barangayId && b.Term_ID == activeTerm.Term_ID);

                    if (barangayBudget == null)
                    {
                        // Create new budget record for this term
                        barangayBudget = new Budget
                        {
                            Barangay_ID = barangayId,
                            Term_ID = activeTerm.Term_ID,
                            budget = amount,
                            balance = amount,
                            disbursed = 0
                        };
                        _context.Budgets.Add(barangayBudget);
                    }
                    else
                    {
                        // Top-up existing budget
                        barangayBudget.budget += amount;
                        barangayBudget.balance += amount;
                        _context.Budgets.Update(barangayBudget);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { success = true, message = "Funds allocated to Barangay successfully." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Error allocating funds: " + ex.Message);
                }
            }
        }
    }
}
