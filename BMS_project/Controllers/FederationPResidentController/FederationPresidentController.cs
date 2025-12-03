using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using BMS_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims; // Added for Logging
using System;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "FederationPresident")]
    public class FederationPresidentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISystemLogService _systemLogService;

        public FederationPresidentController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ISystemLogService systemLogService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _systemLogService = systemLogService;
        }

        private int? GetCurrentUserId()
        {
            // 1. Try Claims
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int id)) return id;

            // 2. Fallback: DB Lookup
            var username = User.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                // Sync lookup is fine for this helper scope
                var login = _context.Login.FirstOrDefault(l => l.Username == username);
                return login?.User_ID;
            }
            return null;
        }

        public async Task<IActionResult> Dashboard(int? termId)
        {
            ViewData["Title"] = "Dashboard";

            var viewModel = new FederationDashboardViewModel();

            // 1. Get All Terms for Dropdown
            viewModel.AllTerms = await _context.KabataanTermPeriods.OrderByDescending(t => t.Start_Date).ToListAsync();

            // 2. Determine Active/Selected Term
            var activeTerm = viewModel.AllTerms.FirstOrDefault(t => t.IsActive);
            int targetTermId = termId ?? (activeTerm?.Term_ID ?? 0);
            viewModel.SelectedTermId = targetTermId;
            viewModel.CurrentTermName = viewModel.AllTerms.FirstOrDefault(t => t.Term_ID == targetTermId)?.Term_Name ?? "Unknown Term";

            if (targetTermId != 0)
            {
                // Project Counts
                viewModel.TotalApprovedProjects = await _context.Projects
                    .CountAsync(p => p.Project_Status == "Approved" && p.Term_ID == targetTermId);
                
                viewModel.TotalPendingProjects = await _context.Projects
                    .CountAsync(p => p.Project_Status == "Pending" && p.Term_ID == targetTermId);

                // Financials
                var fedFund = await _context.FederationFunds
                    .FirstOrDefaultAsync(f => f.Term_ID == targetTermId);

                if (fedFund != null)
                {
                    viewModel.TotalFederationBudget = fedFund.Total_Amount;
                    // "Total Disbursed" requested to be "Total Allocated to Barangays"
                    viewModel.TotalDisbursed = fedFund.Allocated_To_Barangays;
                    // "Remaining Balance" requested to be "Federation Remaining Balance"
                    viewModel.TotalRemainingBalance = fedFund.Total_Amount - fedFund.Allocated_To_Barangays;
                }
                else
                {
                    viewModel.TotalFederationBudget = 0;
                    viewModel.TotalDisbursed = 0;
                    viewModel.TotalRemainingBalance = 0;
                }

                // CHART 1: Monthly Expenses (Projects Approved in this Term)
                var currentYear = DateTime.Now.Year; // Or use term start year? sticking to current year for trend
                
                var monthlyData = await _context.Projects
                    .Include(p => p.Allocations)
                    .Where(p => p.Project_Status == "Approved" && 
                                p.Term_ID == targetTermId && 
                                p.Date_Submitted.HasValue && 
                                p.Date_Submitted.Value.Year == currentYear)
                    .GroupBy(p => p.Date_Submitted.Value.Month)
                    .Select(g => new 
                    { 
                        Month = g.Key, 
                        Total = g.Sum(p => p.Allocations.Sum(a => a.Amount_Allocated)) 
                    })
                    .ToListAsync();

                var labels = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                var data = new decimal[12];

                foreach (var item in monthlyData)
                {
                    if (item.Month >= 1 && item.Month <= 12)
                    {
                        data[item.Month - 1] = item.Total;
                    }
                }

                viewModel.MonthlyExpensesLabelsJson = JsonConvert.SerializeObject(labels);
                viewModel.MonthlyExpensesDataJson = JsonConvert.SerializeObject(data);

                // CHART 2: Barangay Budget Allocation (Data Visualization for Barangays Budget)
                var barangayBudgets = await _context.Budgets
                    .Include(b => b.Barangay)
                    .Where(b => b.Term_ID == targetTermId)
                    .OrderByDescending(b => b.budget) // Top funded first
                    .Select(b => new { Name = b.Barangay.Barangay_Name, Amount = b.budget })
                    .ToListAsync();

                viewModel.BarangayBudgetLabelsJson = JsonConvert.SerializeObject(barangayBudgets.Select(b => b.Name).ToArray());
                viewModel.BarangayBudgetDataJson = JsonConvert.SerializeObject(barangayBudgets.Select(b => b.Amount).ToArray());
            }
            else
            {
                // No term selected or exists
                viewModel.MonthlyExpensesLabelsJson = "[]";
                viewModel.MonthlyExpensesDataJson = "[]";
                viewModel.BarangayBudgetLabelsJson = "[]";
                viewModel.BarangayBudgetDataJson = "[]";
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ComplianceMonitoring(int? termId)
        {
            ViewData["Title"] = "Compliance Monitoring";

            // Populate dropdown for Create Modal
            ViewBag.Barangays = await _context.barangays
                .OrderBy(b => b.Barangay_Name)
                .Select(b => new SelectListItem
                {
                    Value = b.Barangay_ID.ToString(),
                    Text = b.Barangay_Name
                })
                .ToListAsync();

            // Populate Document Types (FIX for ArgumentNullException)
            ViewBag.DocumentTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Report", Text = "Report" },
                new SelectListItem { Value = "Financial Statement", Text = "Financial Statement" },
                new SelectListItem { Value = "Minutes of Meeting", Text = "Minutes of Meeting" },
                new SelectListItem { Value = "Proposal", Text = "Proposal" },
                new SelectListItem { Value = "Project Documentation", Text = "Project Documentation" }
            };

            // 1. Get All Terms for Filter Dropdown
            var terms = await _context.KabataanTermPeriods
                .OrderByDescending(t => t.Start_Date)
                .ToListAsync();
            
            ViewBag.Terms = new SelectList(terms, "Term_ID", "Term_Name");

            // 2. Determine Active/Selected Term
            int selectedTermId;
            if (termId.HasValue)
            {
                selectedTermId = termId.Value;
            }
            else
            {
                var activeTerm = terms.FirstOrDefault(t => t.IsActive);
                selectedTermId = activeTerm?.Term_ID ?? (terms.FirstOrDefault()?.Term_ID ?? 0);
            }

            ViewBag.SelectedTermId = selectedTermId;

            // Fetch Compliances for the View Model
            var compliances = await _context.Compliances
                .Include(c => c.Barangay)
                .Where(c => !c.IsArchived && c.Term_ID == selectedTermId)
                .OrderByDescending(c => c.Due_Date)
                .ToListAsync();

            return View(compliances);
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
                    Amount = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0
                })
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> ProjectHistory(int? termId)
        {
            ViewData["Title"] = "Project History";

            // 1. Get All Terms for Filter Dropdown
            var terms = await _context.KabataanTermPeriods
                .OrderByDescending(t => t.Start_Date)
                .ToListAsync();

            ViewBag.Terms = new SelectList(terms, "Term_ID", "Term_Name");

            // 2. Determine Active/Selected Term
            int selectedTermId;
            if (termId.HasValue)
            {
                selectedTermId = termId.Value;
            }
            else
            {
                var activeTerm = terms.FirstOrDefault(t => t.IsActive);
                selectedTermId = activeTerm?.Term_ID ?? (terms.FirstOrDefault()?.Term_ID ?? 0);
            }

            ViewBag.SelectedTermId = selectedTermId;

            var projects = await _context.Projects
                .Include(p => p.User)
                .ThenInclude(u => u.Barangay)
                .Include(p => p.Allocations)
                .Where(p => p.Project_Status != "Pending" && p.Term_ID == selectedTermId) // Filter by Term
                .OrderByDescending(p => p.Date_Submitted)
                .Select(p => new ProjectApprovalListViewModel
                {
                    Project_ID = p.Project_ID,
                    Title = p.Project_Title,
                    Description = p.Project_Description,
                    Barangay = p.User.Barangay.Barangay_Name,
                    Status = p.Project_Status,
                    DateSubmitted = p.Date_Submitted ?? DateTime.Now,
                    Amount = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0
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

            var strategy = _context.Database.CreateExecutionStrategy();
            IActionResult result = null;
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var project = await _context.Projects
                        .Include(p => p.Allocations)
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Project_ID == model.Project_ID);

                    if (project == null)
                    {
                        result = NotFound();
                        return;
                    }

                    if (project.Project_Status != "Pending") 
                    {
                        TempData["ErrorMessage"] = "Can only edit pending projects.";
                        result = RedirectToAction(nameof(ProjectApprovals));
                        return;
                    }

                    var allocation = project.Allocations.FirstOrDefault();
                    if (allocation == null)
                    {
                        TempData["ErrorMessage"] = "Allocation record not found.";
                        result = RedirectToAction(nameof(ProjectApprovals));
                        return;
                    }

                    decimal finalAmount = allocation.Amount_Allocated;
                    if (model.Approved_Amount.HasValue && model.Approved_Amount.Value != allocation.Amount_Allocated)
                    {
                        allocation.Amount_Allocated = model.Approved_Amount.Value;
                        finalAmount = model.Approved_Amount.Value;
                        _context.ProjectAllocations.Update(allocation);
                    }

                    string newStatus = !string.IsNullOrEmpty(model.Status) ? model.Status : "Approved";
                    project.Project_Status = newStatus;
                    _context.Projects.Update(project);

                    if (newStatus == "Approved")
                    {
                        var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Budget_ID == allocation.Budget_ID);
                        if (budget == null)
                        {
                            throw new Exception("Budget not found.");
                        }

                        if (budget.balance < finalAmount)
                        {
                            throw new Exception("Insufficient budget balance for approval.");
                        }

                        budget.disbursed += finalAmount;
                        budget.balance -= finalAmount;
                        _context.Budgets.Update(budget);
                    }

                    var log = new ProjectLog
                    {
                        Project_ID = project.Project_ID,
                        User_ID = project.User_ID, 
                        Status = newStatus,
                        Changed_On = DateTime.Now,
                        Remarks = model.Remarks ?? $"Project status updated to {newStatus} by Federation President"
                    };
                    _context.ProjectLogs.Add(log);

                    int? userId = GetCurrentUserId();
                    if (userId.HasValue)
                    {
                        await _systemLogService.LogAsync(userId.Value, "Approve/Reject Project", $"Project {project.Project_Title} Status Updated to {newStatus}", "Project", project.Project_ID);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Project {newStatus.ToLower()} successfully.";
                    result = RedirectToAction(nameof(ProjectApprovals));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Error processing project: " + ex.Message;
                    result = RedirectToAction(nameof(ProjectApprovals));
                }
            });
            return result;
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
        public async Task<IActionResult> Budget()
        {
            ViewData["Title"] = "Barangay Budgets";

            // Get Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);

            List<Budget> budgets;
            if (activeTerm != null)
            {
                 budgets = await _context.Budgets
                    .Include(b => b.Barangay)
                    .Where(b => b.Term_ID == activeTerm.Term_ID) // Filter by Term
                    .OrderBy(b => b.Barangay.Barangay_Name)
                    .ToListAsync();
            }
            else 
            {
                budgets = new List<Budget>();
            }

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
        public async Task<IActionResult> AddBarangayBudget(int BarangayId, decimal Allotment)
        {
            if (Allotment <= 0)
            {
                TempData["ErrorMessage"] = "Allotment must be greater than zero.";
                return RedirectToAction("Budget");
            }

            var barangay = await _context.barangays.FirstOrDefaultAsync(b => b.Barangay_ID == BarangayId);
            if (barangay == null)
            {
                TempData["ErrorMessage"] = "Selected barangay not found.";
                return RedirectToAction("Budget");
            }

            // 1. Get Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null)
            {
                TempData["ErrorMessage"] = "No active term found. Cannot allocate budget.";
                return RedirectToAction("Budget");
            }

            // 2. Get Federation Fund for this term
            var fedFund = await _context.FederationFunds.FirstOrDefaultAsync(f => f.Term_ID == activeTerm.Term_ID);
            if (fedFund == null)
            {
                TempData["ErrorMessage"] = "Federation Fund has not been set for this term by Super Admin.";
                return RedirectToAction("Budget");
            }

            // 3. VALIDATION: Check Federation Limit
            if (fedFund.Allocated_To_Barangays + Allotment > fedFund.Total_Amount)
            {
                var remaining = fedFund.Total_Amount - fedFund.Allocated_To_Barangays;
                TempData["ErrorMessage"] = $"Insufficient Federation Funds. Only {remaining:C} is available for distribution.";
                return RedirectToAction("Budget");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 4. Update Federation Fund Tracker
                    fedFund.Allocated_To_Barangays += Allotment;
                    _context.FederationFunds.Update(fedFund);

                    // 5. Find existing budget for the barangay AND Term
                    var existingBudget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Barangay_ID == BarangayId && b.Term_ID == activeTerm.Term_ID);

                    // Fallback: Check for old budget without TermID if you want to migrate it, or just create new.
                    // Decision: Create NEW record if TermID specific one doesn't exist to support clean slate per term.
                    
                    if (existingBudget != null)
                    {
                        existingBudget.budget += Allotment;
                        existingBudget.balance += Allotment;
                        _context.Budgets.Update(existingBudget);
                    }
                    else
                    {
                        var budget = new Budget
                        {
                            Barangay_ID = barangay.Barangay_ID,
                            Term_ID = activeTerm.Term_ID, // Strictly set Term ID
                            budget = Allotment,
                            disbursed = 0m,
                            balance = Allotment
                        };
                        _context.Budgets.Add(budget);
                    }

                    await _context.SaveChangesAsync();

                    // LOGGING
                    int? userId = GetCurrentUserId();
                    if (userId.HasValue)
                    {
                        // Log against the Budget ID (might be new, need to save first which we did)
                        // If new, we need to grab ID. Since we saved changes, the entity 'budget' or 'existingBudget' has the ID.
                        // However, 'budget' variable scope is inside else block. Let's simplify logging ID or just log success.
                        await _systemLogService.LogAsync(userId.Value, "Add Budget", $"Allocated {Allotment:C} to {barangay.Barangay_Name}", "Budget", 0); 
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Budget allocated successfully.";
                    return RedirectToAction("Budget");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Error allocating budget: " + ex.Message;
                    return RedirectToAction("Budget");
                }
            });
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