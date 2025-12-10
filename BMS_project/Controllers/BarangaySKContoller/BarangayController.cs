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
            // 1. Date Validation: Ensure start date is before end date
            if (model.Start_Date.HasValue && model.End_Date.HasValue && model.Start_Date > model.End_Date)
            {
                ModelState.AddModelError("Start_Date", "Start Date cannot be later than End Date.");
            }

            // 2. Date Validation: Ensure start date is not in the past
            if (model.Start_Date.HasValue && model.Start_Date.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError("Start_Date", "Start Date must be in the future    .");
            }

            // 3. Date Validation: Ensure end date is not in the past
            if (model.End_Date.HasValue && model.End_Date.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError("End_Date", "End Date must be later than today.");
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "Project submission failed. Please address the following issues: " + errors + ".";
                return RedirectToAction(nameof(Projects));
            }

            if (model.UploadedFiles == null || model.UploadedFiles.Count == 0)
            {
                TempData["ErrorMessage"] = "Please upload at least one project document (PDF).";
                return RedirectToAction(nameof(Projects));
            }

            foreach (var file in model.UploadedFiles)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".pdf" || file.ContentType != "application/pdf")
                {
                    TempData["ErrorMessage"] = "Only PDF files are allowed.";
                    return RedirectToAction(nameof(Projects));
                }
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

            // 4.1 Date Overlap Validation: Check if the new project overlaps with an existing project
            if (model.Start_Date.HasValue && model.End_Date.HasValue)
            {
                bool dateOverlap = await _context.Projects
                    .Include(p => p.User)
                    .Where(p => p.User.Barangay_ID == user.Barangay_ID && !p.IsArchived && p.Project_Status != "Rejected")
                    .AnyAsync(p => p.Start_Date != null && p.End_Date != null &&
                                   p.Start_Date <= model.End_Date && p.End_Date >= model.Start_Date);

                if (dateOverlap)
                {
                    ModelState.AddModelError("Start_Date", "Project dates overlap with an existing project.");
                    TempData["ErrorMessage"] = "You cannot add a project that overlaps with the dates of an existing project.";
                    return RedirectToAction(nameof(Projects));
                }
            }

            // 4. Uniqueness Validation: Check if a project with the same title exists in the user's Barangay
            // We check this after retrieving the user to ensure we scope the uniqueness to their Barangay.
            bool titleExists = await _context.Projects
                .Include(p => p.User)
                .AnyAsync(p => p.Project_Title == model.Project_Title && p.User.Barangay_ID == user.Barangay_ID);

            if (titleExists)
            {
                ModelState.AddModelError("Project_Title", "A project with this title already exists.");
                TempData["ErrorMessage"] = "A project with this title already exists in your Barangay.";
                return RedirectToAction(nameof(Projects));
            }

            // 5. Get Active Term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null)
            {
                TempData["ErrorMessage"] = "No active term found in the system.";
                return RedirectToAction(nameof(Projects));
            }

            // 6. Get Budget for Active Term
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Barangay_ID == user.Barangay_ID && b.Term_ID == activeTerm.Term_ID);

            if (budget == null)
            {
                TempData["ErrorMessage"] = $"No budget found for this Barangay for the current term ({activeTerm.Term_Name}).";
                return RedirectToAction(nameof(Projects));
            }

            // PART C Logic: Barangay SK - Create Project Validation
            // Validation: Check (Pending Allocations + New Amount) vs Balance
            decimal pendingAllocations = await _context.Projects
                .Where(p => p.User.Barangay_ID == user.Barangay_ID 
                            && p.Term_ID == activeTerm.Term_ID 
                            && p.Project_Status == "Pending" 
                            && !p.IsArchived)
                .SelectMany(p => p.Allocations)
                .SumAsync(a => a.Amount_Allocated);

            if (pendingAllocations + model.Allocated_Amount > budget.balance)
            {
                decimal availableForNew = budget.balance - pendingAllocations;
                // Ensure we don't show negative available if pending > balance (shouldn't happen normally)
                if (availableForNew < 0) availableForNew = 0;

                var phCulture = new System.Globalization.CultureInfo("en-PH");
                TempData["ErrorMessage"] = string.Format(phCulture, "Barangay funds insufficient. Balance: {0:C}, Pending Requests: {1:C}. Available for new projects: {2:C}.", budget.balance, pendingAllocations, availableForNew);
                return RedirectToAction(nameof(Projects));
            }

            // Use Execution Strategy for retries
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                List<string> savedFilePaths = new List<string>();
                bool transactionCommitted = false;

                try
                {
                    _logger.LogInformation("Starting project creation transaction for user {UserId}", user.User_ID);

                    var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                        ? _webHostEnvironment.WebRootPath
                        : _webHostEnvironment.ContentRootPath;

                    string uploadFolder = Path.Combine(baseFolder, "UploadedFiles");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                        _logger.LogInformation("Created upload directory: {UploadFolder}", uploadFolder);
                    }

                    int mainFileId = 0;
                    List<int> allFileIds = new List<int>();

                    // 1. Process all files
                    foreach (var file in model.UploadedFiles)
                    {
                        string safeDocName = Path.GetFileNameWithoutExtension(file.FileName);
                        safeDocName = string.Join("_", safeDocName.Split(Path.GetInvalidFileNameChars()));

                        var extension2 = Path.GetExtension(file.FileName).ToLowerInvariant();
                        string uniqueFileName = $"{safeDocName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{extension2}";
                        string savedFilePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await file.CopyToAsync(stream);
                        }
                        savedFilePaths.Add(savedFilePath);

                        var fileUpload = new FileUpload
                        {
                            User_ID = user.User_ID,
                            File_Name = file.FileName,
                            File = "/UploadedFiles/" + uniqueFileName,
                            Timestamp = DateTime.Now
                        };
                        _context.FileUploads.Add(fileUpload);
                        await _context.SaveChangesAsync(); // Save to get ID

                        allFileIds.Add(fileUpload.File_ID);
                        if (mainFileId == 0) mainFileId = fileUpload.File_ID; // First file is main
                    }

                    // 2. Create Project with Main File_ID
                    var project = new Project
                    {
                        User_ID = user.User_ID,
                        Project_Title = model.Project_Title,
                        Project_Description = model.Project_Description,
                        Date_Submitted = DateTime.Now,
                        Project_Status = "Pending",
                        Start_Date = model.Start_Date,
                        End_Date = model.End_Date,
                        Term_ID = budget.Term_ID,
                        File_ID = mainFileId // Main attachment
                    };

                    _context.Projects.Add(project);
                    await _context.SaveChangesAsync(); // Save to get Project_ID

                    // 3. Create ProjectDocuments
                    foreach (var fid in allFileIds)
                    {
                        var projDoc = new ProjectDocument
                        {
                            Project_ID = project.Project_ID,
                            File_ID = fid,
                            Date_Added = DateTime.Now
                        };
                        _context.ProjectDocuments.Add(projDoc);
                    }
                    await _context.SaveChangesAsync();

                    // 4. Create Allocation
                    var allocation = new ProjectAllocation
                    {
                        Budget_ID = budget.Budget_ID,
                        Project_ID = project.Project_ID,
                        Amount_Allocated = model.Allocated_Amount
                    };
                    _context.ProjectAllocations.Add(allocation);

                    // 5. Create Log
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
                    await _systemLogService.LogAsync(user.User_ID, "Create Project", $"Created Project: {project.Project_Title} with {allFileIds.Count} documents", "Project", project.Project_ID);

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

                    foreach (var path in savedFilePaths)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            try
                            {
                                System.IO.File.Delete(path);
                                _logger.LogInformation("Cleaned up orphaned file {Path}", path);
                            }
                            catch (Exception cleanupEx)
                            {
                                _logger.LogWarning(cleanupEx, "Failed to delete orphaned file");
                            }
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

        public async Task<IActionResult> Dashboard(int? termId)
        {
            ViewData["Title"] = "Dashboard";

            var barangayId = GetBarangayIdFromClaims();
            var vm = new DashboardViewModel();

            // Populate Term Dropdown
            vm.AllTerms = await _context.KabataanTermPeriods.OrderByDescending(t => t.Start_Date).ToListAsync();

            // Determine Selected Term (Default to Active)
            var activeTerm = vm.AllTerms.FirstOrDefault(t => t.IsActive);
            int targetTermId = termId ?? (activeTerm?.Term_ID ?? 0);
            vm.SelectedTermId = targetTermId;
            vm.CurrentTerm = vm.AllTerms.FirstOrDefault(t => t.Term_ID == targetTermId)?.Term_Name ?? "Unknown Term";

            // Default values if no barangay
            if (!barangayId.HasValue)
            {
                return View(vm);
            }

            // 1. KPI Stats
            // Youth (Currently showing All Active regardless of term, as Youth table has no Term_ID)
            vm.TotalYouth = await _context.YouthMembers
                .CountAsync(y => y.Barangay_ID == barangayId.Value && !y.IsArchived);

            // Projects - Filtered by Selected Term
            var projects = await _context.Projects
                .Include(p => p.User)
                .Where(p => p.User.Barangay_ID == barangayId.Value && !p.IsArchived && p.Term_ID == targetTermId)
                .ToListAsync();

            vm.TotalApprovedProjects = projects.Count(p => p.Project_Status == "Approved" || p.Project_Status == "Completed");
            vm.TotalPendingProjects = projects.Count(p => p.Project_Status == "Pending");
            
            // Logic for "Ongoing": Approved projects that haven't ended yet OR simply "Approved" status if "Completed" is used for finished ones.
            // Assuming "Approved" means active/ongoing and "Completed" means done.
            vm.TotalOngoingProjects = projects.Count(p => p.Project_Status == "Approved");

            // 2. Sex Distribution Chart (Youth - Current Snapshot)
            var sexData = await _context.YouthMembers
                .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                .GroupBy(y => y.Sex)
                .Select(g => new { Sex = g.Key, Count = g.Count() })
                .ToListAsync();

            var sexLabels = sexData.Select(d => d.Sex).ToArray();
            var sexCounts = sexData.Select(d => d.Count).ToArray();
            vm.SexDistributionLabels = Newtonsoft.Json.JsonConvert.SerializeObject(sexLabels);
            vm.SexDistributionData = Newtonsoft.Json.JsonConvert.SerializeObject(sexCounts);

            // 3. Age Distribution Chart (Youth - Current Snapshot)
            var youthList = await _context.YouthMembers
                .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                .Select(y => y.Age)
                .ToListAsync();

            var ageGroups = youthList.GroupBy(a => a).OrderBy(g => g.Key);
            vm.AgeDistributionLabels = Newtonsoft.Json.JsonConvert.SerializeObject(ageGroups.Select(g => g.Key.ToString()).ToArray());
            vm.AgeDistributionData = Newtonsoft.Json.JsonConvert.SerializeObject(ageGroups.Select(g => g.Count()).ToArray());

            // 4. Sitio Distribution Chart (Youth per Sitio - Current Snapshot)
            var sitioData = await _context.YouthMembers
                .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                .Include(y => y.Sitio)
                .GroupBy(y => y.Sitio != null ? y.Sitio.Sitio_Name : "Unassigned")
                .Select(g => new { Sitio = g.Key, Count = g.Count() })
                .ToListAsync();

            vm.SitioDistributionLabels = Newtonsoft.Json.JsonConvert.SerializeObject(sitioData.Select(d => d.Sitio).ToArray());
            vm.SitioDistributionData = Newtonsoft.Json.JsonConvert.SerializeObject(sitioData.Select(d => d.Count).ToArray());

            // 5. Project Status Pie Chart
            var projectStatusData = new[] { vm.TotalApprovedProjects, vm.TotalOngoingProjects, vm.TotalPendingProjects };
            vm.ProjectStatusData = Newtonsoft.Json.JsonConvert.SerializeObject(projectStatusData);

            // 6. Calendar Events (Approved Projects - Filtered by Term)
            var calendarEvents = projects
                .Where(p => p.Project_Status == "Approved" || p.Project_Status == "Completed")
                .Select(p => new 
                { 
                    title = p.Project_Title, 
                    start = p.Start_Date?.ToString("yyyy-MM-dd"), 
                    end = p.End_Date?.AddDays(1).ToString("yyyy-MM-dd"), // FullCalendar exclusive end date
                    color = p.Project_Status == "Approved" ? "#28a745" : "#6c757d" // Green for approved, Grey for completed
                })
                .ToList();

            vm.CalendarEvents = Newtonsoft.Json.JsonConvert.SerializeObject(calendarEvents);

            // 7. Compliance Alert System
            // Check for overdue and not submitted compliance items for this Barangay
            var overdueCompliance = await _context.Compliances
                .Where(c => c.Barangay_ID == barangayId.Value && 
                            !c.IsArchived && 
                            c.Status == "Not Submitted" && 
                            c.Due_Date <= DateTime.Today)
                .Select(c => new { c.Title, c.Due_Date })
                .ToListAsync();

            if (overdueCompliance.Any())
            {
                ViewBag.OverdueCompliance = overdueCompliance;
            }

            return View(vm);
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
                    .Include(y => y.Sitio) // Include Sitio navigation
                    .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                    .ToList();

                ViewBag.ArchivedYouth = _context.YouthMembers
                    .Include(y => y.Sitio)
                    .Where(y => y.Barangay_ID == barangayId.Value && y.IsArchived)
                    .ToList();

                // Populate Dropdown List
                var sitios = _context.Sitios
                    .Where(s => s.Barangay_ID == barangayId.Value)
                    .OrderBy(s => s.Sitio_Name)
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                    { 
                        Value = s.Sitio_ID.ToString(), 
                        Text = s.Sitio_Name 
                    })
                    .ToList();

                ViewBag.SitioList = sitios;

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

            // Get Active Term for filtering main project list
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            int activeTermId = activeTerm?.Term_ID ?? 0; // Default to 0 if no active term

            // Fetch ALL non-archived projects for the current user first
            var allUserProjects = await _context.Projects
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
                    Term_ID = p.Term_ID, // Ensure Term_ID is mapped
                    Allocated_Budget = p.Allocations.FirstOrDefault() != null ? p.Allocations.FirstOrDefault().Amount_Allocated : 0
                })
                .ToListAsync();

            // 1. Projects for the Current Active Term (for the main list)
            var projects = allUserProjects
                .Where(p => p.Term_ID == activeTermId)
                .ToList();

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

            // 2. Identify Carry-Over Candidates (from ALL user projects, excluding current term projects)
            // Logic: Active Status (Approved/Ongoing) AND Belong to an Inactive/Old Term
            var carryOverCandidates = allUserProjects
                .Where(p => (p.Project_Status == "Approved" || p.Project_Status == "Ongoing") 
                            && p.Term_ID != activeTermId 
                            && p.Term_ID.HasValue)
                .ToList();

            ViewBag.CarryOverCandidates = carryOverCandidates;

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(int Project_ID, string Project_Status)
        {
            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Project_ID == Project_ID);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Project not found.";
                return RedirectToAction(nameof(Projects));
            }

            // Rule: Cannot edit Pending projects (they are awaiting Fed approval)
            if (project.Project_Status == "Pending")
            {
                TempData["ErrorMessage"] = "Pending projects cannot be edited. Please wait for approval.";
                return RedirectToAction(nameof(Projects));
            }

            // Rule: Approved projects can be updated to Ongoing or Completed
            // Rule: Ongoing projects can be updated to Completed
            if (project.Project_Status == "Approved" || project.Project_Status == "Ongoing")
            {
                // Allow transitions:
                // Approved -> Ongoing
                // Approved -> Completed
                // Ongoing -> Completed
                
                if (Project_Status == "Ongoing" || Project_Status == "Completed")
                {
                    // Optional: Prevent regression from Ongoing to Approved if desired, but basic logic allows switching
                    project.Project_Status = Project_Status;
                    
                    // Auto-generate Compliance if Completed
                    if (Project_Status == "Completed" && project.User != null && project.User.Barangay_ID != null && project.Term_ID != null)
                    {
                        string complianceTitle = $"Completion Report: {project.Project_Title}";
                        
                        bool exists = await _context.Compliances.AnyAsync(c => 
                            c.Barangay_ID == project.User.Barangay_ID && 
                            c.Term_ID == project.Term_ID &&
                            c.Title == complianceTitle);

                        if (!exists)
                        {
                            var compliance = new Compliance
                            {
                                Barangay_ID = project.User.Barangay_ID,
                                Title = complianceTitle,
                                Type = "Project Completion Report",
                                Status = "Not Submitted",
                                Due_Date = DateTime.Now.AddDays(15),
                                Term_ID = project.Term_ID.Value,
                                IsArchived = false
                            };
                            _context.Compliances.Add(compliance);
                            _logger.LogInformation("Auto-generated compliance record for project {ProjectId}", project.Project_ID);
                        }
                    }

                    // Log
                    int? userId = GetCurrentUserId();
                    if (userId.HasValue)
                    {
                        await _systemLogService.LogAsync(userId.Value, "Update Project", $"Updated Project {project.Project_Title} status to {Project_Status}", "Project", project.Project_ID);
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Project marked as {Project_Status}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid status update.";
                }
            }
            else
            {
                // If already completed or rejected
                TempData["ErrorMessage"] = "This project status cannot be changed.";
            }

            return RedirectToAction(nameof(Projects));
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
                .Include(c => c.TemplateFile)
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
                    SubmissionFileId = c.File_ID,
                    TemplateFileName = c.TemplateFile != null ? c.TemplateFile.File_Name : null,
                    TemplateFileId = c.TemplateFile != null ? c.TemplateFile.File_ID : (int?)null
                })
                .OrderByDescending(c => c.DueDate)
                .ToListAsync();

            return View(compliances);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitCompliance(int ComplianceId, List<IFormFile> SubmissionFiles)
        {
            if (SubmissionFiles == null || SubmissionFiles.Count == 0)
            {
                TempData["ErrorMessage"] = "Please select at least one file to upload.";
                return RedirectToAction(nameof(Compliance));
            }

            foreach (var file in SubmissionFiles)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".pdf" || file.ContentType != "application/pdf")
                {
                    TempData["ErrorMessage"] = "Only PDF files are allowed.";
                    return RedirectToAction(nameof(Compliance));
                }
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    var barangayId = GetBarangayIdFromClaims();
                    if (!barangayId.HasValue) 
                    {
                        TempData["ErrorMessage"] = "Unauthorized action.";
                        return; // Or throw/redirect
                    }

                    var compliance = await _context.Compliances.FirstOrDefaultAsync(c => c.Compliance_ID == ComplianceId && c.Barangay_ID == barangayId.Value);
                    if (compliance == null)
                    {
                        TempData["ErrorMessage"] = "Compliance requirement not found.";
                        return;
                    }

                    var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                       ? _webHostEnvironment.WebRootPath
                       : _webHostEnvironment.ContentRootPath;

                    string uploadFolder = Path.Combine(baseFolder, "UploadedFiles", "Submissions");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var userId = GetCurrentUserId();

                    foreach (var file in SubmissionFiles)
                    {
                        string uniqueFileName = $"{compliance.Compliance_ID}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}.pdf";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create FileUpload record
                        var fileUpload = new FileUpload
                        {
                            User_ID = userId,
                            File_Name = file.FileName,
                            File = "/UploadedFiles/Submissions/" + uniqueFileName,
                            Timestamp = DateTime.Now
                        };
                        _context.FileUploads.Add(fileUpload);
                        await _context.SaveChangesAsync();

                        // Create ComplianceDocument record
                        var doc = new ComplianceDocument
                        {
                            Compliance_ID = compliance.Compliance_ID,
                            File_ID = fileUpload.File_ID,
                            Status = "Pending",
                            Date_Submitted = DateTime.Now
                        };
                        _context.ComplianceDocuments.Add(doc);
                    }

                    // Update Compliance Parent Status
                    compliance.Date_Submitted = DateTime.Now;
                    compliance.Status = "Pending"; // Indicates it's under review
                    
                    _context.Compliances.Update(compliance);

                    // Log
                    if (userId.HasValue)
                    {
                         await _systemLogService.LogAsync(userId.Value, "Submit Compliance", $"Submitted {SubmissionFiles.Count} documents for: {compliance.Title}", "Compliance", compliance.Compliance_ID);
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();
                    TempData["SuccessMessage"] = "Compliance documents submitted successfully!";
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "Error submitting compliance");
                    TempData["ErrorMessage"] = "Error submitting compliance: " + ex.Message;
                }
            });

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
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "Unauthorized action.";
                    return RedirectToAction(nameof(Projects));
                }
                
                var user = await _context.Users.FindAsync(userId);
                if (user?.Barangay_ID == null)
                {
                    TempData["ErrorMessage"] = "User is not assigned to a Barangay.";
                    return RedirectToAction(nameof(Projects));
                }

                // 2. Get Project
                var project = await _context.Projects
                    .Include(p => p.Allocations)
                    .FirstOrDefaultAsync(p => p.Project_ID == projectId);

                if (project == null)
                {
                    TempData["ErrorMessage"] = "Project not found.";
                    return RedirectToAction(nameof(Projects));
                }

                // 3. Security: Ensure project belongs to user's barangay
                var projectCreator = await _context.Users.FindAsync(project.User_ID);
                if (projectCreator == null || projectCreator.Barangay_ID != user.Barangay_ID)
                {
                    TempData["ErrorMessage"] = "You are not authorized to decide on this project.";
                    return RedirectToAction(nameof(Projects));
                }

                if (decision == "Terminate")
                {
                    project.Project_Status = "Rejected";
                    project.IsArchived = true;
                    
                    // Log
                    await _systemLogService.LogAsync(user.User_ID, "Terminate Project", $"Rejected and Archived Project: {project.Project_Title}", "Project", project.Project_ID);
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Project terminated and archived successfully.";
                    return RedirectToAction(nameof(Projects));
                }
                else if (decision == "Continue")
                {
                    // 4. Continue Logic
                    // Get Active Term
                    var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
                    if (activeTerm == null)
                    {
                        TempData["ErrorMessage"] = "No active term found. Cannot continue project.";
                        return RedirectToAction(nameof(Projects));
                    }

                    // Get Active Budget (New Term's Budget)
                    var budget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Barangay_ID == user.Barangay_ID && b.Term_ID == activeTerm.Term_ID);

                    if (budget == null)
                    {
                        TempData["ErrorMessage"] = "No budget found for the new term. Please request allocation from Federation President.";
                        return RedirectToAction(nameof(Projects));
                    }

                    // Check Funds
                    if (budget.balance < project.Estimated_Cost)
                    {
                        TempData["ErrorMessage"] = "Insufficient funds in new term to continue this project.";
                        return RedirectToAction(nameof(Projects));
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
                    TempData["SuccessMessage"] = "Project carried over successfully.";
                    return RedirectToAction(nameof(Projects));
                }

                TempData["ErrorMessage"] = "Invalid decision.";
                return RedirectToAction(nameof(Projects));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecideOnCarryOver");
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Projects));
            }
        }
    }
}