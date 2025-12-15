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

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var viewModel = new FederationDashboardViewModel();

            // 1. Get Active Term (ignoring selection logic)
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            
            // If no active term, maybe fallback to latest? For now, logic implies active term.
            // If strictly active:
            int targetTermId = activeTerm?.Term_ID ?? 0;
            viewModel.SelectedTermId = targetTermId;
            viewModel.CurrentTermName = activeTerm?.Term_Name ?? "No Active Term";

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

        public async Task<IActionResult> ComplianceMonitoring()
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
                new SelectListItem { Value = "Summary", Text = "Summary" },
                new SelectListItem { Value = "Liquidation", Text = "Liquidation" },
                new SelectListItem { Value = "Expenses", Text = "Expenses" }
            };

            // 1. Get Active Term only
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            int selectedTermId = activeTerm?.Term_ID ?? 0;

            ViewBag.SelectedTermId = selectedTermId;

            // Fetch Compliances for the View Model
            var compliances = await _context.Compliances
                .Include(c => c.Barangay)
                .Where(c => !c.IsArchived && c.Term_ID == selectedTermId)
                .OrderByDescending(c => c.Due_Date)
                .ToListAsync();

            // Get all approved projects for Generation of Reports section
            var approvedProjects = await _context.Projects
                .Include(p => p.User)
                    .ThenInclude(u => u.Barangay)
                .Where(p => p.Project_Status == "Approved" && p.Term_ID == selectedTermId)
                .OrderByDescending(p => p.Date_Submitted)
                .ToListAsync();

            ViewBag.ApprovedProjects = approvedProjects;

            return View(compliances);
        }

        [HttpGet]
        public async Task<IActionResult> GetComplianceDetails(int id)
        {
            var compliance = await _context.Compliances
                .Include(c => c.Barangay)
                .Include(c => c.Documents)
                    .ThenInclude(d => d.File)
                .FirstOrDefaultAsync(c => c.Compliance_ID == id);

            if (compliance == null) return NotFound();

            // Get submitted documents from ComplianceDocuments
            var submittedDocuments = compliance.Documents
                .Where(d => d.File != null)
                .Select(d => new
                {
                    DocumentId = d.Document_ID,
                    FileName = d.File.File_Name,
                    FileUrl = d.File.File,
                    Status = d.Status,
                    Remarks = d.Remarks ?? "",
                    DateSubmitted = d.Date_Submitted
                })
                .ToList();

            var viewModel = new ComplianceDetailsViewModel
            {
                ComplianceId = compliance.Compliance_ID,
                Title = compliance.Title,
                BarangayName = compliance.Barangay?.Barangay_Name ?? "Unknown",
                ComplianceType = compliance.Type,
                AnnexType = compliance.Annex_Type,
                DueDate = compliance.Due_Date,
                ComplianceStatus = compliance.Status
            };

            return Ok(new
            {
                complianceId = viewModel.ComplianceId,
                title = viewModel.Title,
                barangayName = viewModel.BarangayName,
                complianceType = viewModel.ComplianceType,
                annexType = viewModel.AnnexType,
                dueDate = viewModel.DueDate,
                complianceStatus = viewModel.ComplianceStatus,
                documents = submittedDocuments
            });
        }

        [HttpPost]
        public async Task<IActionResult> ReviewDocument(int documentId, string status, string remarks)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    var doc = await _context.ComplianceDocuments
                        .Include(d => d.Compliance)
                        .FirstOrDefaultAsync(d => d.Document_ID == documentId);

                    if (doc == null) return NotFound("Document not found");

                    doc.Status = status;
                    doc.Remarks = remarks;

                    _context.ComplianceDocuments.Update(doc);

                    // Optional: Update Parent Compliance Status logic?
                    // For now, we just track per document. 
                    // But if all docs are Approved, maybe set Compliance to Approved?
                    // If any is Rejected, maybe set Compliance to "Needs Revision"?
                    
                    var allDocs = await _context.ComplianceDocuments
                        .Where(d => d.Compliance_ID == doc.Compliance_ID)
                        .ToListAsync();

                    // Logic Refined:
                    // 1. If ANY document is Rejected, the entire compliance is "Rejected".
                    // 2. If ANY document is Pending (and none rejected), the review is incomplete -> Status: "Pending".
                    // 3. If NO Pending and NO Rejected, and ALL are Approved -> Status: "Approved".
                    // 4. If NO Pending, NO Rejected and at least ONE is Approved -> Status: "Completed".

                    if (allDocs.Any(d => d.Status == "Rejected"))
                    {
                        doc.Compliance.Status = "Rejected";
                    }
                    else if (allDocs.Any(d => d.Status == "Pending"))
                    {
                        doc.Compliance.Status = "Pending";
                    }
                    else if (allDocs.All(d => d.Status == "Approved"))
                    {
                        doc.Compliance.Status = "Approved";
                    }
                    else if (allDocs.Any(d => d.Status == "Approved"))
                    {
                        doc.Compliance.Status = "Completed";
                    }
                    else
                    {
                        doc.Compliance.Status = "Returned";
                    }
                    
                    _context.Compliances.Update(doc.Compliance);

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectBarangayDocuments(int id)
        {
            // Get all documents for this project uploaded by Barangay SK users (Role_ID = 3)
            var documents = await _context.ProjectDocuments
                .Include(pd => pd.File)
                    .ThenInclude(f => f.User)
                .Include(pd => pd.Project)
                    .ThenInclude(p => p.User)
                .Where(pd => pd.Project_ID == id && 
                             pd.File.User.Role_ID == 3) // Barangay SK role
                .Select(pd => new
                {
                    FileName = pd.File.File_Name,
                    FileUrl = pd.File.File,
                    Description = pd.Description ?? "Document",
                    DateAdded = pd.Date_Added
                })
                .OrderByDescending(d => d.DateAdded)
                .ToListAsync();

            return Ok(new { documents });
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectFederationDocuments(int id)
        {
            // Get all documents for this project uploaded by Federation President (Role_ID = 2)
            var documents = await _context.ProjectDocuments
                .Include(pd => pd.File)
                    .ThenInclude(f => f.User)
                .Include(pd => pd.Project)
                    .ThenInclude(p => p.User)
                .Where(pd => pd.Project_ID == id && 
                             pd.File.User.Role_ID == 2 &&  // Federation President role
                             pd.Description != "Project Proposal") // Exclude the original project proposal
                .Select(pd => new
                {
                    FileName = pd.File.File_Name,
                    FileUrl = pd.File.File,
                    Description = pd.Description ?? "Document",
                    DateAdded = pd.Date_Added
                })
                .OrderByDescending(d => d.DateAdded)
                .ToListAsync();

            return Ok(new { documents });
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

        public async Task<IActionResult> ProjectHistory(int? barangayId, string status)
        {
            ViewData["Title"] = "Project History";

            // Populate Barangay Dropdown
            var barangays = await _context.barangays.OrderBy(b => b.Barangay_Name).ToListAsync();
            ViewBag.Barangays = barangays.Select(b => new SelectListItem
            {
                Value = b.Barangay_ID.ToString(),
                Text = b.Barangay_Name,
                Selected = barangayId.HasValue && b.Barangay_ID == barangayId.Value
            }).ToList();

            // Populate Status Dropdown
            var statusList = new List<string> { "Approved", "Rejected", "Completed", "Ongoing" };
            ViewBag.Statuses = statusList.Select(s => new SelectListItem
            {
                Value = s,
                Text = s,
                Selected = !string.IsNullOrEmpty(status) && s.Equals(status, StringComparison.OrdinalIgnoreCase)
            }).ToList();

            ViewBag.SelectedBarangayId = barangayId;
            ViewBag.SelectedStatus = status;

            var query = _context.Projects
                .Include(p => p.User)
                .ThenInclude(u => u.Barangay)
                .Include(p => p.Allocations)
                .Where(p => p.Project_Status != "Pending"); // Base filter: only processed projects

            if (barangayId.HasValue)
            {
                query = query.Where(p => p.User.Barangay_ID == barangayId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Project_Status == status.Trim());
            }

            var projects = await query
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

        // New Action: Project Documents - Display Annex Documents
        public async Task<IActionResult> ProjectDocuments(int? barangayId)
        {
            ViewData["Title"] = "Project Documents";

            // Populate Barangay Dropdown
            var barangays = await _context.barangays.OrderBy(b => b.Barangay_Name).ToListAsync();
            ViewBag.Barangays = barangays.Select(b => new SelectListItem
            {
                Value = b.Barangay_ID.ToString(),
                Text = b.Barangay_Name,
                Selected = barangayId.HasValue && b.Barangay_ID == barangayId.Value
            }).ToList();

            ViewBag.SelectedBarangayId = barangayId;

            // Query ProjectDocuments that have annex types (not "Project Proposal" and not "Additional Document")
            // Exclude files uploaded by Federation President (Role_ID = 2) with Annex I or Annex H
            var query = _context.ProjectDocuments
                .Include(pd => pd.Project)
                .ThenInclude(p => p.User)
                .ThenInclude(u => u.Barangay)
                .Include(pd => pd.File)
                .ThenInclude(f => f.User)
                .Where(pd => pd.Description != null && 
                             pd.Description != "Project Proposal" && 
                             pd.Description != "Additional Document" &&
                             !(pd.File.User.Role_ID == 2 && (pd.Description == "Annex I" || pd.Description == "Annex H"))); // Exclude Federation uploads

            if (barangayId.HasValue)
            {
                query = query.Where(pd => pd.Project.User.Barangay_ID == barangayId.Value);
            }

            var documents = await query
                .OrderByDescending(pd => pd.Date_Added)
                .Select(pd => new
                {
                    ProjectId = pd.Project.Project_ID,
                    ProjectTitle = pd.Project.Project_Title,
                    ProjectDescription = pd.Project.Project_Description,
                    BarangayName = pd.Project.User.Barangay.Barangay_Name,
                    AnnexType = pd.Description,
                    FileName = pd.File.File_Name,
                    FilePath = pd.File.File,
                    UploadedBy = pd.File.User != null ? pd.File.User.First_Name + " " + pd.File.User.Last_Name : null,
                    DateAdded = pd.Date_Added,
                    ProjectStatus = pd.Project.Project_Status,
                    UploadedFilesCount = _context.ProjectDocuments
                        .Count(d => d.Project_ID == pd.Project.Project_ID && d.File.User.Role_ID == 2 && (d.Description == "Annex I" || d.Description == "Annex H"))
                })
                .ToListAsync();

            return View(documents);
        }

        // New Action for AJAX Fetch
        [HttpGet]
        public async Task<IActionResult> GetProjectDetails(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Allocations)
                .Include(p => p.Documents)
                    .ThenInclude(d => d.File)
                .Include(p => p.User)
                    .ThenInclude(u => u.Barangay)
                .FirstOrDefaultAsync(p => p.Project_ID == id);

            if (project == null)
            {
                return NotFound();
            }

            var amount = project.Allocations.FirstOrDefault()?.Amount_Allocated ?? 0;
            
            // Map Documents - Only show "Project Proposal" documents (Barangay SK submissions)
            var documents = project.Documents
                .Where(d => d.Description == "Project Proposal")
                .Select(d => new 
                {
                    id = d.Document_ID,
                    fileName = d.File?.File_Name ?? "Unknown",
                    fileId = d.File_ID,
                    dateAdded = d.Date_Added.ToString("yyyy-MM-dd")
                }).ToList();

            // Handle legacy single file attachment (File_ID on Project table)
            // If there are no entries in project_document table, check the legacy File_ID column
            if (!documents.Any() && project.File_ID != 0)
            {
                var legacyFile = await _context.FileUploads.FindAsync(project.File_ID);
                if (legacyFile != null)
                {
                    documents.Add(new 
                    {
                        id = 0, // No document ID
                        fileName = legacyFile.File_Name,
                        fileId = legacyFile.File_ID,
                        dateAdded = project.Date_Submitted?.ToString("yyyy-MM-dd") ?? ""
                    });
                }
            }

            return Json(new
            {
                title = project.Project_Title,
                description = project.Project_Description,
                barangay = project.User?.Barangay?.Barangay_Name ?? "Unknown",
                amount = amount,
                dateSubmitted = project.Date_Submitted?.ToString("yyyy-MM-dd") ?? "-",
                status = project.Project_Status,
                documents = documents
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
                            .ThenInclude(a => a.Budget)
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
                        // Try to fetch allocation directly from database as a fallback
                        allocation = await _context.ProjectAllocations
                            .FirstOrDefaultAsync(pa => pa.Project_ID == model.Project_ID);
                        
                        if (allocation == null)
                        {
                            TempData["ErrorMessage"] = $"Allocation record not found for project '{project.Project_Title}'. This project may have been created incorrectly. Please contact the administrator.";
                            result = RedirectToAction(nameof(ProjectApprovals));
                            return;
                        }
                    }

                    decimal finalAmount = allocation.Amount_Allocated;
                    decimal originalAmount = allocation.Amount_Allocated;
                    bool amountChanged = false;
                    
                    if (model.Approved_Amount.HasValue && model.Approved_Amount.Value != allocation.Amount_Allocated)
                    {
                        allocation.Amount_Allocated = model.Approved_Amount.Value;
                        finalAmount = model.Approved_Amount.Value;
                        amountChanged = true;
                        _context.ProjectAllocations.Update(allocation);
                    }

                    // Determine status: If only budget changed and no status provided, keep as Pending
                    string newStatus;
                    if (!string.IsNullOrEmpty(model.Status))
                    {
                        newStatus = model.Status;
                    }
                    else if (amountChanged)
                    {
                        newStatus = "Pending"; // Keep as pending if only budget was changed
                    }
                    else
                    {
                        newStatus = project.Project_Status; // Keep current status
                    }
                    
                    project.Project_Status = newStatus;
                    _context.Projects.Update(project);

                    if (newStatus == "Approved")
                    {
                        var budget = await _context.Budgets
                            .Include(b => b.Barangay)
                            .FirstOrDefaultAsync(b => b.Budget_ID == allocation.Budget_ID);
                        
                        if (budget == null)
                        {
                            throw new Exception("Budget not found.");
                        }

                        // Validation: Check if approved amount exceeds remaining balance
                        if (budget.balance < finalAmount)
                        {
                            var phCulture = new System.Globalization.CultureInfo("en-PH");
                            TempData["ErrorMessage"] = string.Format(phCulture, 
                                "Insufficient budget balance for {0}. Requested: {1:C}, Available: {2:C}", 
                                budget.Barangay?.Barangay_Name ?? "this barangay",
                                finalAmount, 
                                budget.balance);
                            result = RedirectToAction(nameof(ProjectApprovals));
                            await transaction.RollbackAsync();
                            return;
                        }

                        budget.disbursed += finalAmount;
                        // budget.balance should NOT be decremented when project is approved
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
                        var phCulture = new System.Globalization.CultureInfo("en-PH");
                        string statusRemark = $"Project '{project.Project_Title}' status updated to {newStatus}";
                        
                        if (amountChanged)
                        {
                            statusRemark += $" - Budget changed from {originalAmount.ToString("C", phCulture)} to {finalAmount.ToString("C", phCulture)} (Change: {(finalAmount - originalAmount).ToString("C", phCulture)})";
                        }
                        
                        await _systemLogService.LogAsync(userId.Value, "Approve/Reject Project", statusRemark, "Project", project.Project_ID);
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

        public async Task<IActionResult> DownloadFile(int id, bool inline = false)
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
            else if (filePath.EndsWith(".xlsx")) contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            else if (filePath.EndsWith(".docx")) contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (filePath.EndsWith(".pptx")) contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            
            string downloadName = !string.IsNullOrEmpty(fileUpload.File_Name) ? fileUpload.File_Name : "document.pdf";

            if (inline)
            {
                Response.Headers.Add("Content-Disposition", $"inline; filename={downloadName}");
                return File(memory, contentType);
            }
            else
            {
                // Use the original file name for download
                if (!downloadName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) downloadName += ".pdf"; // Ensure extension for download
                return File(memory, contentType, downloadName);
            }
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
        public async Task<IActionResult> AddBarangayBudget(int BarangayId, decimal Allotment, decimal InitialBalance = 0)
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
                var phCulture = new System.Globalization.CultureInfo("en-PH");
                TempData["ErrorMessage"] = string.Format(phCulture, "Insufficient Federation Funds. Only {0:C} is available for distribution.", remaining);
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
                    
                    int budgetId = 0;
                    decimal oldBudget = 0;
                    decimal newBudget = 0;
                    string logAction = "";
                    
                    if (existingBudget != null)
                    {
                        oldBudget = existingBudget.budget;
                        existingBudget.budget += Allotment;
                        existingBudget.balance += (Allotment * 0.10m) + InitialBalance;
                        existingBudget.InitialBalance += InitialBalance; // Store initial balance
                        newBudget = existingBudget.budget;
                        budgetId = existingBudget.Budget_ID;
                        logAction = "Update Budget";
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
                            balance = (Allotment * 0.10m) + InitialBalance,
                            InitialBalance = InitialBalance // Store initial balance
                        };
                        _context.Budgets.Add(budget);
                        await _context.SaveChangesAsync(); // Save to get Budget_ID
                        budgetId = budget.Budget_ID;
                        newBudget = Allotment;
                        logAction = "Add Budget";
                    }

                    if (logAction != "Add Budget")
                    {
                        await _context.SaveChangesAsync();
                    }

                    // LOGGING
                    int? userId = GetCurrentUserId();
                    if (userId.HasValue)
                    {
                        var phCulture = new System.Globalization.CultureInfo("en-PH");
                        string remark = logAction == "Add Budget" 
                            ? $"Added budget to {barangay.Barangay_Name}: {Allotment.ToString("C", phCulture)} (New Total: {newBudget.ToString("C", phCulture)})"
                            : $"Updated budget for {barangay.Barangay_Name}: Added {Allotment.ToString("C", phCulture)} (Previous: {oldBudget.ToString("C", phCulture)}, New: {newBudget.ToString("C", phCulture)})";
                        
                        await _systemLogService.LogAsync(userId.Value, logAction, remark, "Budget", budgetId); 
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

        // Add Initial Balance to existing Barangay SK Budget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFederationInitialBalance(int BarangayId, decimal InitialBalance)
        {
            if (InitialBalance <= 0)
            {
                TempData["ErrorMessage"] = "Initial balance must be greater than zero.";
                return RedirectToAction("Budget");
            }

            var barangay = await _context.barangays.FirstOrDefaultAsync(b => b.Barangay_ID == BarangayId);
            if (barangay == null)
            {
                TempData["ErrorMessage"] = "Selected barangay not found.";
                return RedirectToAction("Budget");
            }

            // Get Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null)
            {
                TempData["ErrorMessage"] = "No active term found.";
                return RedirectToAction("Budget");
            }

            // Find existing budget for the barangay and active term
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Barangay_ID == BarangayId && b.Term_ID == activeTerm.Term_ID);

            if (budget == null)
            {
                TempData["ErrorMessage"] = "No budget found for this barangay. Please add a budget first.";
                return RedirectToAction("Budget");
            }

            try
            {
                // Store old values for logging
                var oldBalance = budget.balance;

                // Add initial balance directly to balance (SK Budget)
                budget.balance += InitialBalance;

                _context.Budgets.Update(budget);
                await _context.SaveChangesAsync();

                // Log the action
                int? userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    var phCulture = new System.Globalization.CultureInfo("en-PH");
                    await _systemLogService.LogAsync(
                        userId.Value, 
                        "Add Initial Balance", 
                        $"Federation President added initial balance of {InitialBalance.ToString("C", phCulture)} to {barangay.Barangay_Name} SK Budget. Previous SK Budget: {oldBalance.ToString("C", phCulture)}, New SK Budget: {budget.balance.ToString("C", phCulture)}", 
                        "Budget", 
                        budget.Budget_ID
                    );
                }

                TempData["SuccessMessage"] = $"Initial balance of ₱{InitialBalance:N2} has been added successfully to {barangay.Barangay_Name}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding initial balance: " + ex.Message;
            }

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
            // budget.balance should NOT be decremented when project is approved

            _context.Budgets.Update(budget);
            _context.SaveChanges();

            return Ok(new { success = true, remaining = budget.balance });
        }

        [HttpGet]
        public async Task<IActionResult> GetDisbursementBreakdown(int budgetId)
        {
            try
            {
                var disbursements = await _context.ProjectAllocations
                    .Where(pa => pa.Budget_ID == budgetId)
                    .Include(pa => pa.Project)
                    .Where(pa => pa.Project.Project_Status == "Approved" || pa.Project.Project_Status == "Completed")
                    .Select(pa => new
                    {
                        projectTitle = pa.Project.Project_Title,
                        status = pa.Project.Project_Status,
                        amount = pa.Amount_Allocated,
                        date = pa.Project.Date_Submitted
                    })
                    .OrderByDescending(x => x.date)
                    .ToListAsync();

                return Json(disbursements);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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

        [HttpPost]
        public async Task<IActionResult> UploadProjectDocuments(int projectId, List<IFormFile> files, string annexType)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return Json(new { success = false, message = "No files selected" });
                }

                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return Json(new { success = false, message = "Project not found" });
                }

                // Get current user ID once at the start
                var userId = GetCurrentUserId();

                const long maxFileSize = 25 * 1024 * 1024; // 25MB

                foreach (var file in files)
                {
                    // Validate file size
                    if (file.Length > maxFileSize)
                    {
                        return Json(new { success = false, message = $"File {file.FileName} exceeds 25MB limit" });
                    }

                    // Validate PDF
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (extension != ".pdf" || file.ContentType != "application/pdf")
                    {
                        return Json(new { success = false, message = $"File {file.FileName} is not a PDF" });
                    }

                    // Save file
                    var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                        ? _webHostEnvironment.WebRootPath
                        : _webHostEnvironment.ContentRootPath;

                    var uploadFolder = Path.Combine(baseFolder, "UploadedFiles", "ProjectDocuments");
                    
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{extension}";
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save to database
                    var fileUpload = new FileUpload
                    {
                        File_Name = file.FileName,
                        File = "UploadedFiles/ProjectDocuments/" + uniqueFileName,
                        User_ID = userId,
                        Timestamp = DateTime.Now
                    };
                    _context.FileUploads.Add(fileUpload);
                    await _context.SaveChangesAsync();

                    var projectDocument = new ProjectDocument
                    {
                        Project_ID = projectId,
                        File_ID = fileUpload.File_ID,
                        Description = annexType ?? "Additional Document",
                        Date_Added = DateTime.Now
                    };
                    _context.ProjectDocuments.Add(projectDocument);
                }

                await _context.SaveChangesAsync();

                // Log the action
                if (userId.HasValue)
                {
                    await _systemLogService.LogAsync(
                        userId.Value,
                        "Upload",
                        $"Uploaded {files.Count} document(s) to Project ID: {projectId}",
                        "ProjectDocuments",
                        projectId
                    );
                }

                return Json(new { success = true, message = $"{files.Count} file(s) uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadedProjectFiles(int projectId)
        {
            try
            {
                var files = await _context.ProjectDocuments
                    .Include(pd => pd.File)
                        .ThenInclude(f => f.User)
                    .Where(pd => pd.Project_ID == projectId 
                        && (pd.Description == "Annex I" || pd.Description == "Annex H")
                        && pd.File.User.Role_ID == 2)
                    .Select(pd => new
                    {
                        FileName = pd.File.File_Name,
                        FileUrl = pd.File.File,
                        DateAdded = pd.Date_Added,
                        AnnexType = pd.Description
                    })
                    .OrderByDescending(f => f.DateAdded)
                    .ToListAsync();

                return Json(new { success = true, files });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectProposalDocuments(int projectId)
        {
            try
            {
                var files = await _context.ProjectDocuments
                    .Include(pd => pd.File)
                    .Where(pd => pd.Project_ID == projectId 
                        && pd.Description == "Project Proposal")
                    .Select(pd => new
                    {
                        FileName = pd.File.File_Name,
                        FileUrl = pd.File.File,
                        DateAdded = pd.Date_Added
                    })
                    .OrderByDescending(f => f.DateAdded)
                    .ToListAsync();

                return Json(new { success = true, files });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        public IActionResult Documents()
        {
            ViewData["Title"] = "Downloadable Template Document";
            return View();
        }
    }
}