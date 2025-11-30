using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "FederationPresident")]
    public class FederationPresidentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FederationPresidentController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var viewModel = new FederationDashboardViewModel();

            // Project Counts
            viewModel.TotalApprovedProjects = await _context.Projects.CountAsync(p => p.Project_Status == "Approved");
            viewModel.TotalPendingProjects = await _context.Projects.CountAsync(p => p.Project_Status == "Pending");

            // Budget Totals
            viewModel.TotalFederationBudget = await _context.Budgets.SumAsync(b => (decimal?)b.budget) ?? 0M;
            viewModel.TotalDisbursed = await _context.Budgets.SumAsync(b => (decimal?)b.disbursed) ?? 0M;
            viewModel.TotalRemainingBalance = await _context.Budgets.SumAsync(b => (decimal?)b.balance) ?? 0M;

            // Chart Data Preparation
            var barangayExpenses = await _context.Budgets
                .Include(b => b.Barangay)
                .Select(b => new BarangayExpense
                {
                    BarangayName = b.Barangay.Barangay_Name,
                    TotalDisbursed = b.disbursed
                })
                .ToListAsync();

            viewModel.BarangayExpenseList = barangayExpenses;
            viewModel.ChartLabelsJson = JsonConvert.SerializeObject(barangayExpenses.Select(be => be.BarangayName).ToList());
            viewModel.ChartDataJson = JsonConvert.SerializeObject(barangayExpenses.Select(be => be.TotalDisbursed).ToList());

            return View(viewModel);
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

        public async Task<IActionResult> ProjectApprovals()
        {
            ViewData["Title"] = "Project Approvals";

            var projects = await _context.Projects
                .Include(p => p.User)
                .ThenInclude(u => u.Barangay)
                .Include(p => p.Files) // Include Files
                .Include(p => p.Allocations) // Include Allocations
                .Where(p => p.Project_Status == "Pending")
                .OrderByDescending(p => p.Date_Submitted)
                .Select(p => new ProjectApprovalListViewModel
                {
                    Project_ID = p.Project_ID,
                    Title = p.Project_Title,
                    Description = p.Project_Description,
                    Barangay = p.User.Barangay.Barangay_Name,
                    Status = p.Project_Status,
                    DateSubmitted = p.Date_Submitted ?? DateTime.Now,
                    // Fix: Populate Amount from Allocation
                    Amount = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0,
                    DocumentPath = p.Files.FirstOrDefault() != null ? p.Files.FirstOrDefault().File : null,
                    DocumentId = p.Files.FirstOrDefault() != null ? p.Files.FirstOrDefault().File_ID : null
                })
                .ToListAsync();

            return View(projects);
        }

        // New Action for AJAX Fetch
        [HttpGet]
        public async Task<IActionResult> GetProjectDetails(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Allocations)
                .FirstOrDefaultAsync(p => p.Project_ID == id);

            if (project == null)
            {
                return NotFound();
            }

            var amount = project.Allocations.FirstOrDefault()?.Amount_Allocated ?? 0;

            return Json(new
            {
                amount = amount,
                status = project.Project_Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProject(ProjectApprovalActionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input.";
                return RedirectToAction(nameof(ProjectApprovals));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Allocations)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Project_ID == model.Project_ID);

                if (project == null)
                {
                    return NotFound();
                }

                // Allow editing if it's Pending (or maybe strictly Pending as per original code)
                // For now, keeping the check but you might want to remove it if you want to edit approved projects.
                if (project.Project_Status != "Pending") 
                {
                     // Only allow editing Pending projects for now
                     TempData["ErrorMessage"] = "Can only edit pending projects.";
                     return RedirectToAction(nameof(ProjectApprovals));
                }

                var allocation = project.Allocations.FirstOrDefault();
                if (allocation == null)
                {
                    TempData["ErrorMessage"] = "Allocation record not found.";
                    return RedirectToAction(nameof(ProjectApprovals));
                }

                // 1. Update Amount if changed
                decimal finalAmount = allocation.Amount_Allocated;
                if (model.Approved_Amount.HasValue && model.Approved_Amount.Value != allocation.Amount_Allocated)
                {
                    allocation.Amount_Allocated = model.Approved_Amount.Value;
                    finalAmount = model.Approved_Amount.Value;
                    _context.ProjectAllocations.Update(allocation);
                }

                // 2. Update Project Status
                string newStatus = !string.IsNullOrEmpty(model.Status) ? model.Status : "Approved";
                project.Project_Status = newStatus;
                _context.Projects.Update(project);

                // 3. Handle Budget Logic (Only if Approved)
                if (newStatus == "Approved")
                {
                    var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Budget_ID == allocation.Budget_ID);
                    if (budget == null)
                    {
                        throw new Exception("Budget not found.");
                    }

                    if (budget.balance < finalAmount)
                    {
                        // Rollback is handled by catch block
                        throw new Exception("Insufficient budget balance for approval.");
                    }

                    budget.disbursed += finalAmount;
                    budget.balance -= finalAmount;
                    _context.Budgets.Update(budget);
                }

                // 4. Log
                var log = new ProjectLog
                {
                    Project_ID = project.Project_ID,
                    User_ID = project.User_ID, 
                    Status = newStatus,
                    Changed_On = DateTime.Now,
                    Remarks = model.Remarks ?? $"Project status updated to {newStatus} by Federation President"
                };
                _context.ProjectLogs.Add(log);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Project {newStatus.ToLower()} successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Error processing project: " + ex.Message;
            }

            return RedirectToAction(nameof(ProjectApprovals));
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var fileUpload = await _context.FileUploads.FindAsync(id);
            if (fileUpload == null)
            {
                return NotFound();
            }

            string filePath;
            
            // Check if it's a legacy path (starts with /UploadedFiles/)
            if (fileUpload.File.StartsWith("/UploadedFiles/"))
            {
                 // Legacy: located in wwwroot
                 // Remove leading slash for Path.Combine
                 filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, fileUpload.File.TrimStart('/'));
            }
            else
            {
                // New: located in UploadedFiles directory in ContentRoot
                string relativePath = fileUpload.File;
                // If the stored path doesn't start with UploadedFiles, prepend it (assuming it's just the filename or relative)
                if (!relativePath.StartsWith("UploadedFiles") && !relativePath.Contains("/") && !relativePath.Contains("\\"))
                {
                     relativePath = System.IO.Path.Combine("UploadedFiles", relativePath);
                }
                // If it is already "UploadedFiles/file.pdf", it's fine.
                
                filePath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            string contentType = "application/pdf"; // Default to PDF as per restriction
            // basic mime type check if needed
            if (filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg")) contentType = "image/jpeg";
            else if (filePath.EndsWith(".png")) contentType = "image/png";
            
            // Use the original file name for download
            string downloadName = !string.IsNullOrEmpty(fileUpload.File_Name) ? fileUpload.File_Name : "document.pdf";
            if (!downloadName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) downloadName += ".pdf";

            return File(memory, contentType, downloadName);
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
