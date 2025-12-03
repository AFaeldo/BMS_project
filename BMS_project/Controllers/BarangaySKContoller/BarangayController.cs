using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using BMS_project.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Claims;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "BarangaySk")]
    public class BarangaySkController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<BarangaySkController> _logger;
        private readonly ISystemLogService _systemLogService;

        public BarangaySkController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<BarangaySkController> logger, ISystemLogService systemLogService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _systemLogService = systemLogService;
        }

        // Helper: read Barangay_ID from claims (same pattern used elsewhere)
        private int? GetBarangayIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Barangay_ID");
            return claim != null && int.TryParse(claim.Value, out int id) ? id : (int?)null;
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
                var login = _context.Login.FirstOrDefault(l => l.Username == username);
                return login?.User_ID;
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(int ProjectId, string DocumentName, IFormFile UploadedFile)
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Projects));
            }

            var extension = Path.GetExtension(UploadedFile.FileName).ToLowerInvariant();
            if (extension != ".pdf" || UploadedFile.ContentType != "application/pdf")
            {
                TempData["ErrorMessage"] = "Only PDF files are allowed.";
                return RedirectToAction(nameof(Projects));
            }

            try
            {
                // Prefer web root so files are served statically and to avoid writing into protected app folders
                var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                    ? _webHostEnvironment.WebRootPath
                    : _webHostEnvironment.ContentRootPath;

                string uploadFolder = Path.Combine(baseFolder, "UploadedFiles");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    _logger.LogInformation("Created upload directory: {UploadFolder}", uploadFolder);
                }

                string safeDocName = string.IsNullOrWhiteSpace(DocumentName)
                    ? "Document"
                    : Path.GetFileNameWithoutExtension(DocumentName);

                // sanitize filename
                safeDocName = string.Join("_", safeDocName.Split(Path.GetInvalidFileNameChars()));

                string uniqueFileName = $"{safeDocName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{extension}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // 5. Database Insert
                var username = User.Identity?.Name;
                var loginRecord = await _context.Login.Include(l => l.User).FirstOrDefaultAsync(l => l.Username == username);
                
                if (loginRecord?.User == null)
                {
                    throw new Exception("User not found or not associated with a profile.");
                }

                var fileUpload = new FileUpload
                {
                    User_ID = loginRecord.User.User_ID,
                    File_Name = DocumentName,
                    File = "UploadedFiles/" + uniqueFileName, // Store relative path
                    Timestamp = DateTime.Now
                };

                _context.FileUploads.Add(fileUpload);
                await _context.SaveChangesAsync();

                // LOGGING
                await _systemLogService.LogAsync(loginRecord.User.User_ID, "Upload Document", $"Uploaded Document: {DocumentName} for Project ID {ProjectId}", "FileUpload", fileUpload.File_ID);

                TempData["SuccessMessage"] = "Document uploaded successfully!";
                _logger.LogInformation("File uploaded: {FilePath} by user {Username}", filePath, username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed for project {ProjectId}", ProjectId);
                TempData["ErrorMessage"] = "File upload failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Projects));
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
                [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
                [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)] // 100 MB
                public async Task<IActionResult> CreateProject(ProjectCreationViewModel model)
                {
                    if (!ModelState.IsValid)
                    {
                        var errors = string.Join("; ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                        TempData["ErrorMessage"] = "Validation failed: " + errors;
                        return RedirectToAction(nameof(Projects));
                    }
        
                    if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                    {
                        TempData["ErrorMessage"] = "Please upload a project document (PDF).";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    var ext = Path.GetExtension(model.UploadedFile.FileName).ToLowerInvariant();
                    if (ext != ".pdf" || model.UploadedFile.ContentType != "application/pdf")
                    {
                        TempData["ErrorMessage"] = "Only PDF files are allowed.";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    var username = User.Identity?.Name;
                    var loginRecord = await _context.Login
                        .Include(l => l.User)
                        .FirstOrDefaultAsync(l => l.Username == username);
        
                    if (loginRecord == null || loginRecord.User == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    var user = loginRecord.User;
                    if (user.Barangay_ID == null)
                    {
                        TempData["ErrorMessage"] = "User is not assigned to a Barangay.";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    var budget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Barangay_ID == user.Barangay_ID);
        
                    if (budget == null)
                    {
                        TempData["ErrorMessage"] = "No budget found for this Barangay.";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    // PART C Logic: Barangay SK - Create Project Validation
                    // Validation: Project Cost cannot exceed BarangayBudget.Balance
                    if (model.Allocated_Amount > budget.balance)
                    {
                        TempData["ErrorMessage"] = $"Barangay funds insufficient. You are trying to allocate {model.Allocated_Amount:C}, but the available balance is {budget.balance:C}.";
                        return RedirectToAction(nameof(Projects));
                    }
        
                    // Use Execution Strategy for retries
                    var strategy = _context.Database.CreateExecutionStrategy();
        
                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        string? savedFilePath = null;
                        bool transactionCommitted = false;
        
                        try
                        {
                            _logger.LogInformation("Starting project creation transaction for user {UserId}", user.User_ID);
        
                            var project = new Project
                            {
                                User_ID = user.User_ID,
                                Project_Title = model.Project_Title,
                                Project_Description = model.Project_Description,
                                Date_Submitted = DateTime.Now,
                                Project_Status = "Pending",
                                Start_Date = model.Start_Date,
                                End_Date = model.End_Date,
                                Term_ID = budget.Term_ID // Ensure project is linked to the active term
                            };
        
                            _context.Projects.Add(project);
                            await _context.SaveChangesAsync();
        
                            var allocation = new ProjectAllocation
                            {
                                Budget_ID = budget.Budget_ID,
                                Project_ID = project.Project_ID,
                                Amount_Allocated = model.Allocated_Amount
                            };
                            _context.ProjectAllocations.Add(allocation);
        
                            // Save file to webroot UploadedFiles
                            var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                                ? _webHostEnvironment.WebRootPath
                                : _webHostEnvironment.ContentRootPath;
        
                            string uploadFolder = Path.Combine(baseFolder, "UploadedFiles");
                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                                _logger.LogInformation("Created upload directory: {UploadFolder}", uploadFolder);
                            }
        
                            string safeDocName = string.IsNullOrWhiteSpace(model.DocumentName)
                                ? "Document"
                                : Path.GetFileNameWithoutExtension(model.DocumentName);
        
                            safeDocName = string.Join("_", safeDocName.Split(Path.GetInvalidFileNameChars()));
        
                            var extension2 = Path.GetExtension(model.UploadedFile.FileName).ToLowerInvariant();
                            string uniqueFileName = $"{safeDocName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{extension2}";
                            savedFilePath = Path.Combine(uploadFolder, uniqueFileName);
        
                            using (var stream = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await model.UploadedFile.CopyToAsync(stream);
                            }
        
                            var fileUpload = new FileUpload
                            {
                                User_ID = user.User_ID,
                                File_Name = model.DocumentName,
                                File = "/UploadedFiles/" + uniqueFileName,
                                Timestamp = DateTime.Now
                            };
                            _context.FileUploads.Add(fileUpload);
        
                            var log = new ProjectLog
                            {
                                Project_ID = project.Project_ID,
                                User_ID = user.User_ID,
                                Status = "Pending",
                                Changed_On = DateTime.Now,
                                Remarks = "Project created and submitted for approval."
                            };
                            _context.ProjectLogs.Add(log);
        
                            // LOGGING (System Log)
                            await _systemLogService.LogAsync(user.User_ID, "Create Project", $"Created Project: {project.Project_Title}", "Project", project.Project_ID);
        
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            transactionCommitted = true;
        
                            _logger.LogInformation("Transaction committed for project {ProjectId}", project.Project_ID);
                            TempData["SuccessMessage"] = "Project submitted successfully!";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "ERROR in CreateProject");
                            if (!transactionCommitted)
                            {
                                try
                                {
                                    await transaction.RollbackAsync();
                                    _logger.LogInformation("Transaction rolled back.");
                                }
                                catch (Exception rbEx)
                                {
                                    _logger.LogError(rbEx, "Rollback failed");
                                }
                            }
        
                            if (savedFilePath != null && System.IO.File.Exists(savedFilePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(savedFilePath);
                                    _logger.LogInformation("Cleaned up orphaned file {Path}", savedFilePath);
                                }
                                catch (Exception cleanupEx)
                                {
                                    _logger.LogWarning(cleanupEx, "Failed to delete orphaned file");
                                }
                            }
        
                            throw; // Re-throw to be caught by strategy or outer handler
                        }
                    });
        
                    if (TempData["SuccessMessage"] != null)
                    {
                        return RedirectToAction(nameof(Projects));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while creating the project.";
                        return RedirectToAction(nameof(Projects));
                    }
                }
        // Download File Action
        public async Task<IActionResult> DownloadFile(int id)
        {
            var fileUpload = await _context.FileUploads.FindAsync(id);
            if (fileUpload == null)
            {
                return NotFound();
            }

            string filePath;

            if (!string.IsNullOrEmpty(fileUpload.File) && (fileUpload.File.StartsWith("/UploadedFiles/") || fileUpload.File.StartsWith("UploadedFiles/")))
            {
                var trimmed = fileUpload.File.TrimStart('/');
                filePath = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, trimmed);
            }
            else
            {
                // fallback: treat DB value as relative to content root
                var relativePath = fileUpload.File ?? string.Empty;
                if (!relativePath.StartsWith("UploadedFiles") && !relativePath.Contains("/") && !relativePath.Contains("\\"))
                {
                    relativePath = Path.Combine("UploadedFiles", relativePath);
                }
                filePath = Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);
            }

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("Requested file not found: {FilePath}", filePath);
                return NotFound("File not found on server.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            string contentType = "application/pdf";
            string downloadName = !string.IsNullOrEmpty(fileUpload.File_Name) ? fileUpload.File_Name : "document.pdf";
            if (!downloadName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) downloadName += ".pdf";

            return File(memory, contentType, downloadName);
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var barangayId = GetBarangayIdFromClaims();
            if (!barangayId.HasValue)
            {
                // If user is not associated with a barangay, return a default budget
                return View(new Budget { budget = 0, disbursed = 0, balance = 0 });
            }

            // 1. Get Active Term
            var activeTerm = await _context.KabataanTermPeriods
                                            .FirstOrDefaultAsync(t => t.IsActive);

            if (activeTerm == null)
            {
                // No active term, return a default budget
                return View(new Budget { budget = 0, disbursed = 0, balance = 0 });
            }

            // 2. Filter Budget by Barangay_ID and Active Term_ID
            var budget = await _context.Budgets
                                        .FirstOrDefaultAsync(b => b.Barangay_ID == barangayId.Value &&
                                                                b.Term_ID == activeTerm.Term_ID);

            // 3. Handle Null: If no budget found for the active term, pass a default one.
            return View(budget ?? new Budget { budget = 0, disbursed = 0, balance = 0 });
        }

        public IActionResult Documents()
        {
            ViewData["Title"] = "Document Uploads";
            return View();
        }

        // YouthProfiles: filter by Barangay_ID claim so each barangay only sees its own youth
        public IActionResult YouthProfiles()
        {
            ViewData["Title"] = "Youth Profiling";

            var barangayId = GetBarangayIdFromClaims();
            if (barangayId.HasValue)
            {
                var youthList = _context.YouthMembers
                    .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                    .ToList();

                ViewBag.ArchivedYouth = _context.YouthMembers
                    .Where(y => y.Barangay_ID == barangayId.Value && y.IsArchived)
                    .ToList();

                return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
            }

            // If no Barangay claim: return empty list to avoid exposing data
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", new List<YouthMember>());
        }

        public async Task<IActionResult> ProjectList()
        {
            ViewData["Title"] = "Project History";

            var username = User.Identity?.Name;
            var loginRecord = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (loginRecord == null || loginRecord.User == null || loginRecord.User.Barangay_ID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Extract the User object explicitly
            BMS_project.Models.User userModel = loginRecord.User; 
            int currentUserId = userModel.User_ID; // Access User_ID from the explicit User model

            var projects = await _context.Projects
                .Include(p => p.User)
                .Include(p => p.Allocations)
                .Where(p => p.User_ID == currentUserId && p.Project_Status != "Pending")
                .OrderByDescending(p => p.Date_Submitted)
                .Select(p => new ProjectListViewModel
                {
                    Project_ID = p.Project_ID,
                    Project_Title = p.Project_Title,
                    Project_Description = p.Project_Description,
                    Start_Date = p.Start_Date,
                    End_Date = p.End_Date,
                    Date_Submitted = p.Date_Submitted,
                    Project_Status = p.Project_Status,
                    Allocated_Budget = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0
                })
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> Projects()
        {
            ViewData["Title"] = "Project Management";

            var username = User.Identity?.Name;
            var loginRecord = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (loginRecord == null || loginRecord.User == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Extract the User object explicitly
            BMS_project.Models.User userModel = loginRecord.User; 
            int currentUserId = userModel.User_ID; // Access User_ID from the explicit User model

            var projects = await _context.Projects
                .Where(p => p.User_ID == currentUserId && !p.IsArchived)
                .Include(p => p.Allocations)
                .OrderByDescending(p => p.Date_Submitted)
                .Select(p => new ProjectListViewModel
                {
                    Project_ID = p.Project_ID,
                    Project_Title = p.Project_Title,
                    Project_Description = p.Project_Description,
                    Start_Date = p.Start_Date,
                    End_Date = p.End_Date,
                    Project_Status = p.Project_Status,
                    Allocated_Budget = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0
                })
                .ToListAsync();

            var archivedProjects = await _context.Projects
                .Where(p => p.User_ID == currentUserId && p.IsArchived)
                .Select(p => new ProjectListViewModel
                {
                    Project_ID = p.Project_ID,
                    Project_Title = p.Project_Title,
                    Project_Description = p.Project_Description,
                    Start_Date = p.Start_Date,
                    End_Date = p.End_Date,
                    Project_Status = p.Project_Status
                })
                .ToListAsync();

            ViewBag.ArchivedProjects = archivedProjects;

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                TempData["ErrorMessage"] = "Project not found.";
                return RedirectToAction(nameof(Projects));
            }

            // Soft delete
            project.IsArchived = true;

            // LOGGING
            int? userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Delete/Archive Project", $"Archived Project ID {id}: {project.Project_Title}", "Project", id);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project archived successfully!";
            return RedirectToAction(nameof(Projects));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreSelected(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var projectsToRestore = await _context.Projects
                    .Where(p => selectedIds.Contains(p.Project_ID))
                    .ToListAsync();

                foreach (var p in projectsToRestore)
                {
                    p.IsArchived = false;
                }

                // LOGGING
                int? userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    await _systemLogService.LogAsync(userId.Value, "Restore Project", $"Restored {projectsToRestore.Count} Projects", "Project", null);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Selected projects restored successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "No projects selected for restoration.";
            }

            return RedirectToAction(nameof(Projects));
        }

        public async Task<IActionResult> Compliance()
        {
            ViewData["Title"] = "Compliance";

            var barangayId = GetBarangayIdFromClaims();
            if (!barangayId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var compliances = await _context.Compliances
                .Include(c => c.SubmissionFile)
                .Where(c => c.Barangay_ID == barangayId.Value && !c.IsArchived)
                .Select(c => new ComplianceViewModel
                {
                    Compliance_ID = c.Compliance_ID,
                    Title = c.Title,
                    Type = c.Type,
                    DueDate = c.Due_Date,
                    Status = c.Status,
                    Date_Submitted = c.Date_Submitted,
                    SubmissionFilePath = c.SubmissionFile != null ? c.SubmissionFile.File : null,
                    SubmissionFileId = c.File_ID
                })
                .OrderByDescending(c => c.DueDate)
                .ToListAsync();

            return View(compliances);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitCompliance(int ComplianceId, IFormFile SubmissionFile)
        {
            if (SubmissionFile == null || SubmissionFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Compliance));
            }

            var ext = Path.GetExtension(SubmissionFile.FileName).ToLowerInvariant();
            if (ext != ".pdf" || SubmissionFile.ContentType != "application/pdf")
            {
                TempData["ErrorMessage"] = "Only PDF files are allowed.";
                return RedirectToAction(nameof(Compliance));
            }

            try
            {
                var barangayId = GetBarangayIdFromClaims();
                if (!barangayId.HasValue) return Unauthorized();

                var compliance = await _context.Compliances.FirstOrDefaultAsync(c => c.Compliance_ID == ComplianceId && c.Barangay_ID == barangayId.Value);
                if (compliance == null)
                {
                    TempData["ErrorMessage"] = "Compliance requirement not found.";
                    return RedirectToAction(nameof(Compliance));
                }

                var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                   ? _webHostEnvironment.WebRootPath
                   : _webHostEnvironment.ContentRootPath;

                string uploadFolder = Path.Combine(baseFolder, "UploadedFiles", "Submissions");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = $"{compliance.Compliance_ID}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}.pdf";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await SubmissionFile.CopyToAsync(stream);
                }

                // Create FileUpload record
                var userId = GetCurrentUserId();
                var fileUpload = new FileUpload
                {
                    User_ID = userId,
                    File_Name = SubmissionFile.FileName,
                    File = "/UploadedFiles/Submissions/" + uniqueFileName,
                    Timestamp = DateTime.Now
                };
                _context.FileUploads.Add(fileUpload);
                await _context.SaveChangesAsync();

                // Update Compliance
                compliance.File_ID = fileUpload.File_ID;
                compliance.Date_Submitted = DateTime.Now;
                compliance.Status = "Pending"; // Or "Submitted", depending on workflow. "Pending" implies pending review.
                
                _context.Compliances.Update(compliance);

                // Log
                if (userId.HasValue)
                {
                     await _systemLogService.LogAsync(userId.Value, "Submit Compliance", $"Submitted compliance for: {compliance.Title}", "Compliance", compliance.Compliance_ID);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Compliance submitted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting compliance");
                TempData["ErrorMessage"] = "Error submitting compliance: " + ex.Message;
            }

            return RedirectToAction(nameof(Compliance));
        }

        public async Task<IActionResult> Budgets()
        {
            ViewData["Title"] = "Budget & Finance";

            var barangayId = GetBarangayIdFromClaims();
            if (!barangayId.HasValue)
            {
                // If user is not associated with a barangay, return a default budget
                return View(new Budget { budget = 0, disbursed = 0, balance = 0 });
            }

            // 1. Get Active Term
            var activeTerm = await _context.KabataanTermPeriods
                                            .FirstOrDefaultAsync(t => t.IsActive);

            if (activeTerm == null)
            {
                // No active term, return a default budget
                return View(new Budget { budget = 0, disbursed = 0, balance = 0 });
            }

            // 2. Filter Budget by Barangay_ID and Active Term_ID
            var budget = await _context.Budgets
                                        .FirstOrDefaultAsync(b => b.Barangay_ID == barangayId.Value &&
                                                                b.Term_ID == activeTerm.Term_ID);

            // 3. Handle Null: If no budget found for the active term, pass a default one.
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecideOnCarryOver(int projectId, string decision)
        {
            try
            {
                // 1. Get User & Barangay
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();
                
                var user = await _context.Users.FindAsync(userId);
                if (user?.Barangay_ID == null) return BadRequest("User has no Barangay.");

                // 2. Get Project
                var project = await _context.Projects
                    .Include(p => p.Allocations)
                    .FirstOrDefaultAsync(p => p.Project_ID == projectId);

                if (project == null) return NotFound("Project not found.");

                // 3. Security: Ensure project belongs to user's barangay
                var projectCreator = await _context.Users.FindAsync(project.User_ID);
                if (projectCreator == null || projectCreator.Barangay_ID != user.Barangay_ID)
                {
                    return Forbid();
                }

                if (decision == "Terminate")
                {
                    project.Project_Status = "Terminated";
                    
                    // Log
                    await _systemLogService.LogAsync(user.User_ID, "Terminate Project", $"Terminated Project: {project.Project_Title}", "Project", project.Project_ID);
                    
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "Project terminated." });
                }
                else if (decision == "Continue")
                {
                    // 4. Continue Logic
                    // Get Active Term
                    var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
                    if (activeTerm == null) return BadRequest("No active term found.");

                    // Get Active Budget (New Term's Budget)
                    var budget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Barangay_ID == user.Barangay_ID && b.Term_ID == activeTerm.Term_ID);

                    if (budget == null) return BadRequest("No budget found for the new term. Please request allocation from Federation President.");

                    // Check Funds
                    if (budget.balance < project.Estimated_Cost)
                    {
                        return BadRequest("Insufficient funds in new term to continue this project.");
                    }

                    // Deduct from New Term Budget
                    budget.balance -= project.Estimated_Cost;
                    // Typically disbursed increases when cash is released, but if we treat allocation as 'reserved', we reduce balance. 
                    // We will assume Balance is what's available for NEW allocations.

                    // Update Project to New Term
                    project.Term_ID = activeTerm.Term_ID;

                    // Create new Allocation Record linked to the NEW Budget
                    var allocation = new ProjectAllocation
                    {
                        Budget_ID = budget.Budget_ID,
                        Project_ID = project.Project_ID,
                        Amount_Allocated = project.Estimated_Cost
                    };
                    _context.ProjectAllocations.Add(allocation);

                    // Keep status as Approved (it was already approved or pending in old term, if pending it should be approved now if funded)
                    project.Project_Status = "Approved"; 

                    // Log
                    await _systemLogService.LogAsync(user.User_ID, "Carry Over Project", $"Carried Over Project: {project.Project_Title} to Term {activeTerm.Term_Name}", "Project", project.Project_ID);

                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "Project carried over successfully." });
                }

                return BadRequest("Invalid decision.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecideOnCarryOver");
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
    }
}