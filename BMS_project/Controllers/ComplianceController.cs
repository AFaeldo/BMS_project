using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using BMS_project.Services;
using System.Security.Claims;

namespace BMS_project.Controllers
{
    [Authorize]
    public class ComplianceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplianceController(ApplicationDbContext context, ISystemLogService systemLogService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _systemLogService = systemLogService;
            _webHostEnvironment = webHostEnvironment;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : (int?)null;
        }

        // 1. Index() (GET)
        [Authorize(Roles = "FederationPresident,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            // Populate ViewBag.Barangays for the modal
            ViewBag.Barangays = await _context.barangays
                .OrderBy(b => b.Barangay_Name)
                .Select(b => new SelectListItem
                {
                    Value = b.Barangay_ID.ToString(),
                    Text = b.Barangay_Name
                })
                .ToListAsync();

            // Populate ViewBag.DocumentTypes
            ViewBag.DocumentTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Report", Text = "Report" },
                new SelectListItem { Value = "Financial Statement", Text = "Financial Statement" },
                new SelectListItem { Value = "Minutes of Meeting", Text = "Minutes of Meeting" },
                new SelectListItem { Value = "Proposal", Text = "Proposal" },
                new SelectListItem { Value = "Project Documentation", Text = "Project Documentation" }
            };

            // Query _context.Compliances
            // Crucial: Use .Include(c => c.Barangay) to display Barangay Name
            var compliances = await _context.Compliances
                .Include(c => c.Barangay)
                .Where(c => !c.IsArchived)
                .OrderByDescending(c => c.Due_Date)
                .ToListAsync();

            // Return the list to the View
            return View(compliances);
        }

        // 2. Create() (GET)
        [HttpGet]
        [Authorize(Roles = "FederationPresident,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            // Query _context.Barangays to get SelectListItem list
            var barangays = await _context.barangays
                .OrderBy(b => b.Barangay_Name)
                .Select(b => new SelectListItem
                {
                    Value = b.Barangay_ID.ToString(),
                    Text = b.Barangay_Name
                })
                .ToListAsync();

            // Populate Document Types
            var documentTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Report", Text = "Report" },
                new SelectListItem { Value = "Financial Statement", Text = "Financial Statement" },
                new SelectListItem { Value = "Minutes of Meeting", Text = "Minutes of Meeting" },
                new SelectListItem { Value = "Proposal", Text = "Proposal" },
                new SelectListItem { Value = "Project Documentation", Text = "Project Documentation" }
            };

            // Pass list into CreateComplianceViewModel
            var model = new CreateComplianceViewModel
            {
                BarangayList = barangays,
                DocumentTypeList = documentTypes,
                DueDate = DateTime.Today.AddDays(7) // Default to 1 week from now
            };

            // Since we are using a shared view with a modal, just redirect to the main monitoring page
            // This GET action is effectively disabled/redirects for safety
            return RedirectToAction("ComplianceMonitoring", "FederationPresident");
        }

        // GET: Compliance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compliance = await _context.Compliances
                .Include(c => c.Barangay)
                .FirstOrDefaultAsync(m => m.Compliance_ID == id);

            if (compliance == null)
            {
                return NotFound();
            }

            return View(compliance);
        }

        // POST: Compliance/Archive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FederationPresident,SuperAdmin")]
        public async Task<IActionResult> Archive(int id)
        {
            var compliance = await _context.Compliances.FindAsync(id);
            if (compliance == null)
            {
                return NotFound();
            }

            compliance.IsArchived = true;
            _context.Compliances.Update(compliance);
            await _context.SaveChangesAsync();

            // Log
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Archive Compliance", $"Archived requirement: {compliance.Title}", "Compliance", compliance.Compliance_ID);
            }

            TempData["SuccessMessage"] = "Compliance record archived successfully.";

            // Redirect based on role or referer?
            // Defaulting to Federation President view as per context
            if (User.IsInRole("FederationPresident"))
            {
                return RedirectToAction("ComplianceMonitoring", "FederationPresident");
            }
            return RedirectToAction(nameof(Index));
        }

                // 3. Create(CreateComplianceViewModel model) (POST)
                [HttpPost]
                [ValidateAntiForgeryToken]
                [Authorize(Roles = "FederationPresident,SuperAdmin")]
                public async Task<IActionResult> Create(CreateComplianceViewModel model)
                {
                    // Custom validation for TemplateFile (due to [Required] on ViewModel)
                    if (model.TemplateFile == null || model.TemplateFile.Length == 0)
                    {
                        ModelState.AddModelError("TemplateFile", "Template file is required.");
                    }
        
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            // Create a new Compliance object
                            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
                            if (activeTerm == null)
                            {
                                 TempData["ErrorMessage"] = "No active term found. Cannot create compliance.";
                                 // Reload dropdowns and return view (handled below in the catch/else block)
                                 ModelState.AddModelError("", "No active term found. Cannot create compliance."); // Add to model state to trigger error path
                            }
        
                            if (ModelState.IsValid) // Re-check after potential activeTerm validation
                            {
                                var compliance = new Compliance
                                {
                                    // Map fields
                                    Barangay_ID = model.SelectedBarangayId,
                                    Title = model.Title,
                                    Type = model.DocumentType,
                                    Due_Date = model.DueDate, 
                                    Term_ID = activeTerm!.Term_ID, // Use ! since we checked ModelState.IsValid after activeTerm check
                                    // Instructions removed
        
                                    // Set defaults
                                    Status = "Not Submitted",
                                    File_ID = null
                                };
        
                                // Handle Template File Upload
                                var baseFolder = !string.IsNullOrEmpty(_webHostEnvironment.WebRootPath)
                                   ? _webHostEnvironment.WebRootPath
                                   : _webHostEnvironment.ContentRootPath;
        
                                string uploadFolder = Path.Combine(baseFolder, "UploadedFiles", "Templates");
                                if (!Directory.Exists(uploadFolder))
                                {
                                    Directory.CreateDirectory(uploadFolder);
                                }
        
                                string uniqueFileName = $"Template_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{Path.GetExtension(model.TemplateFile!.FileName)}"; // ! because it's required
                                string filePath = Path.Combine(uploadFolder, uniqueFileName);
        
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await model.TemplateFile.CopyToAsync(stream);
                                }
        
                                var templateUpload = new FileUpload
                                {
                                    User_ID = GetCurrentUserId(),
                                    File_Name = model.TemplateFile.FileName,
                                    File = "/UploadedFiles/Templates/" + uniqueFileName,
                                    Timestamp = DateTime.Now
                                };
                                _context.FileUploads.Add(templateUpload);
                                await _context.SaveChangesAsync();
        
                                compliance.TemplateFile_ID = templateUpload.File_ID;
                                
                                // Save Compliance
                                _context.Add(compliance);
                                await _context.SaveChangesAsync();
        
                                // Log
                                var userId = GetCurrentUserId();
                                if (userId.HasValue)
                                {
                                    await _systemLogService.LogAsync(userId.Value, "Create Compliance", $"Created requirement: {model.Title}", "Compliance", compliance.Compliance_ID);
                                }
        
                                TempData["SuccessMessage"] = "Compliance requirement created successfully.";
                                return RedirectToAction("ComplianceMonitoring", "FederationPresident");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log exception (console or logger)
                            Console.WriteLine("Error creating compliance: " + ex.Message);
                            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            TempData["ErrorMessage"] = "Error saving to database: " + innerMessage;
                            ModelState.AddModelError("", "An unexpected error occurred: " + innerMessage); // Add to model state to trigger error path
                        }
                    }
                    // If ModelState is not valid, or an exception occurred, or active term not found
                    // Reload dropdowns and return view
                    ViewBag.Barangays = await _context.barangays
                        .OrderBy(b => b.Barangay_Name)
                        .Select(b => new SelectListItem
                        {
                            Value = b.Barangay_ID.ToString(),
                            Text = b.Barangay_Name
                        })
                        .ToListAsync();
        
                    ViewBag.DocumentTypes = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "Report", Text = "Report" },
                        new SelectListItem { Value = "Financial Statement", Text = "Financial Statement" },
                        new SelectListItem { Value = "Minutes of Meeting", Text = "Minutes of Meeting" },
                        new SelectListItem { Value = "Proposal", Text = "Proposal" },
                        new SelectListItem { Value = "Project Documentation", Text = "Project Documentation" }
                    };
        
                    // Reload Terms for the filter on the main page
                    var terms = await _context.KabataanTermPeriods
                        .OrderByDescending(t => t.Start_Date)
                        .ToListAsync();
                    ViewBag.Terms = new SelectList(terms, "Term_ID", "Term_Name");
                    
                    // Default selected term (active)
                    var activeTermForView = terms.FirstOrDefault(t => t.IsActive);
                    ViewBag.SelectedTermId = activeTermForView?.Term_ID ?? 0;
        
                    // Capture validation errors for TempData if not already done
                    if (!ModelState.IsValid && TempData["ErrorMessage"] == null)
                    {
                         var errors = string.Join("; ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                         TempData["ErrorMessage"] = "Validation failed: " + errors;
                    }
                    
                    // Return to the Federation view
                    return View("~/Views/FederationPresident/ComplianceMonitoring.cshtml", await _context.Compliances.Include(c => c.Barangay).OrderByDescending(c => c.Due_Date).ToListAsync()); 
                }
        // Helper for API if needed still
        [HttpGet]
        public async Task<IActionResult> GetCompliancesForFederation(int? barangayId)
        {
            var query = _context.Compliances
                .Include(c => c.Barangay)
                .AsQueryable();

            if (barangayId.HasValue && barangayId.Value > 0)
            {
                query = query.Where(c => c.Barangay_ID == barangayId.Value);
            }

            var data = await query
                .OrderBy(c => c.Barangay.Barangay_Name)
                .ThenByDescending(c => c.Due_Date)
                .Select(c => new
                {
                    c.Compliance_ID,
                    BarangayName = c.Barangay.Barangay_Name,
                    c.Title,
                    c.Type,
                    c.Status,
                    DueDate = c.Due_Date,
                    c.Date_Submitted,
                    c.File_ID,
                    SubmissionFileId = c.File_ID
                })
                .ToListAsync();

            return Json(data);
        }
        
        // Download action
        public async Task<IActionResult> Download(int fileId)
        {
            var fileUpload = await _context.FileUploads.FindAsync(fileId);
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
    }
}