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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BarangaySkController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

            // 1. Validation: Check for PDF
            var extension = System.IO.Path.GetExtension(UploadedFile.FileName).ToLowerInvariant();
            if (extension != ".pdf" || UploadedFile.ContentType != "application/pdf")
            {
                TempData["ErrorMessage"] = "Only PDF files are allowed.";
                return RedirectToAction(nameof(Projects));
            }

            try
            {
                // 2. Directory Setup
                string uploadFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "UploadedFiles");
                if (!System.IO.Directory.Exists(uploadFolder))
                {
                    System.IO.Directory.CreateDirectory(uploadFolder);
                }

                // 3. File Naming
                // Sanitize DocumentName or use a default if empty
                string safeDocName = string.IsNullOrWhiteSpace(DocumentName) 
                    ? "Document" 
                    : string.Join("_", DocumentName.Split(System.IO.Path.GetInvalidFileNameChars()));
                
                // Format: Name_Timestamp.pdf
                string uniqueFileName = $"{safeDocName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                string filePath = System.IO.Path.Combine(uploadFolder, uniqueFileName);

                // 4. Save File
                using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // 5. Database Insert
                // Need User_ID for the record
                var username = User.Identity.Name;
                var user = await _context.Login.Include(l => l.User).FirstOrDefaultAsync(l => l.Username == username);
                int? userId = user?.User?.User_ID;

                var fileUpload = new FileUpload
                {
                    Project_ID = ProjectId,
                    User_ID = userId,
                    File_Name = DocumentName,
                    File = "/UploadedFiles/" + uniqueFileName,
                    Timestamp = DateTime.Now
                };

                _context.FileUploads.Add(fileUpload); // Assuming DbSet<FileUpload> is named FileUploads
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Document uploaded successfully!";
            }
            catch (Exception ex)
            {
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

            // Early Validation for File Upload
            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                var extension = System.IO.Path.GetExtension(model.UploadedFile.FileName).ToLowerInvariant();
                if (extension != ".pdf" || model.UploadedFile.ContentType != "application/pdf")
                {
                    TempData["ErrorMessage"] = "Only PDF files are allowed.";
                    return RedirectToAction(nameof(Projects));
                }
            }
            else
            {
                 TempData["ErrorMessage"] = "Please upload a project document (PDF).";
                 return RedirectToAction(nameof(Projects));
            }

            var username = User.Identity.Name;
            var login = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (login == null || login.User == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Projects));
            }

            var user = login.User;
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

            if (model.Allocated_Amount > budget.balance)
            {
                TempData["ErrorMessage"] = $"Insufficient funds. Available balance: {budget.balance:C}";
                return RedirectToAction(nameof(Projects));
            }

            // Start Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            string savedFilePath = null;
            bool transactionCommitted = false;

            try
            {
                Console.WriteLine("Starting project creation transaction...");

                // 1. Create Project
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
                Console.WriteLine($"Project created with ID: {project.Project_ID}");

                // 2. Create Allocation
                var allocation = new ProjectAllocation
                {
                    Budget_ID = budget.Budget_ID,
                    Project_ID = project.Project_ID,
                    Amount_Allocated = model.Allocated_Amount
                };
                _context.ProjectAllocations.Add(allocation);
                Console.WriteLine("Project allocation added.");

                // 3. Handle File Upload
                if (_webHostEnvironment.WebRootPath == null)
                {
                    // Fallback if WebRootPath is null
                    _webHostEnvironment.WebRootPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
                }

                string uploadFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "UploadedFiles");
                if (!System.IO.Directory.Exists(uploadFolder))
                {
                    System.IO.Directory.CreateDirectory(uploadFolder);
                    Console.WriteLine($"Created upload directory: {uploadFolder}");
                }

                string safeDocName = string.IsNullOrWhiteSpace(model.DocumentName) 
                    ? "Document" 
                    : string.Join("_", model.DocumentName.Split(System.IO.Path.GetInvalidFileNameChars()));
                
                var extension = System.IO.Path.GetExtension(model.UploadedFile.FileName).ToLowerInvariant();
                string uniqueFileName = $"{safeDocName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                savedFilePath = System.IO.Path.Combine(uploadFolder, uniqueFileName);

                Console.WriteLine($"Saving file to: {savedFilePath}");
                using (var stream = new System.IO.FileStream(savedFilePath, System.IO.FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }
                Console.WriteLine("File saved successfully.");

                var fileUpload = new FileUpload
                {
                    Project_ID = project.Project_ID,
                    User_ID = user.User_ID,
                    File_Name = model.DocumentName,
                    File = "/UploadedFiles/" + uniqueFileName,
                    Timestamp = DateTime.Now
                };
                _context.FileUploads.Add(fileUpload);

                // 4. Log the action
                var log = new ProjectLog
                {
                    Project_ID = project.Project_ID,
                    User_ID = user.User_ID,
                    Status = "Pending",
                    Changed_On = DateTime.Now,
                    Remarks = "Project created and submitted for approval."
                };
                _context.ProjectLogs.Add(log);

                // 5. Final Save & Commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                transactionCommitted = true;
                Console.WriteLine("Transaction committed successfully.");

                TempData["SuccessMessage"] = "Project submitted successfully!";
                return RedirectToAction(nameof(Projects));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateProject: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Rollback transaction ONLY if not committed
                if (!transactionCommitted)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine("Transaction rolled back.");
                    }
                    catch (Exception rollbackEx)
                    {
                        Console.WriteLine($"Rollback failed: {rollbackEx.Message}");
                    }
                }

                // Clean up the uploaded file if it was created
                if (savedFilePath != null && System.IO.File.Exists(savedFilePath))
                {
                    try 
                    { 
                        System.IO.File.Delete(savedFilePath);
                        Console.WriteLine("Cleaned up orphaned file.");
                    } 
                    catch 
                    { 
                        // Ignore cleanup errors
                    }
                }

                TempData["ErrorMessage"] = "An error occurred while creating the project: " + ex.Message;
                return RedirectToAction(nameof(Projects));
            }
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
            Console.WriteLine("Entering Projects GET action...");
            ViewData["Title"] = "Project Management";

            var username = User.Identity.Name;
            Console.WriteLine($"Current User: {username}");

            var login = await _context.Login
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Username == username);

            if (login == null || login.User == null)
            {
                Console.WriteLine("Login or User not found.");
                // Handle the case where user is not found (though Authorize attribute should prevent this mostly)
                return RedirectToAction("Login", "Account");
            }

            Console.WriteLine($"Fetching projects for UserID: {login.User.User_ID}");
            
            // Map to ViewModel to avoid circular references in View
            var projects = await _context.Projects
                .Where(p => p.User_ID == login.User.User_ID)
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
            
            Console.WriteLine($"Found {projects.Count} projects.");

            Console.WriteLine("Returning View(projects)...");
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