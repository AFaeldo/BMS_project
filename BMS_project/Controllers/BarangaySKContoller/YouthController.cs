using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

namespace BMS_project.Controllers.BarangaySKContoller
{
    public class YouthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public YouthController(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        // Helper to get Barangay_ID from claims
        private int? GetBarangayIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Barangay_ID");
            return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
        }

        private int? GetCurrentUserId()
        {
             var claim = User.FindFirst(ClaimTypes.NameIdentifier);
             if (claim != null && int.TryParse(claim.Value, out int id)) return id;
             return null;
        }

        // ✅ Show the YouthProfiles page
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult YouthProfiles()
        {
            var barangayId = GetBarangayIdFromClaims();

            // If the user has a valid Barangay_ID, filter the youth list
            if (barangayId.HasValue)
            {
                var youthList = _context.YouthMembers
                    .Include(y => y.Sitio)
                    .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                    .ToList();
                
                ViewBag.ArchivedYouth = _context.YouthMembers
                    .Include(y => y.Sitio)
                    .Where(y => y.Barangay_ID == barangayId.Value && y.IsArchived)
                    .ToList();

                // Populate Dropdown
                ViewBag.SitioList = _context.Sitios
                    .Where(s => s.Barangay_ID == barangayId.Value)
                    .OrderBy(s => s.Sitio_Name)
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = s.Sitio_ID.ToString(), Text = s.Sitio_Name })
                    .ToList();

                return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
            }

            // Fallback (or if user has no barangay): return empty list
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", new List<YouthMember>());
        }

        // ✅ Add a new youth member (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(YouthMember member)
        {
            // 1. Automation: Assign Barangay_ID from logged-in user claims
            var barangayId = GetBarangayIdFromClaims();

            if (barangayId.HasValue)
            {
                member.Barangay_ID = barangayId.Value;
            }
            else
            {
                // If no Barangay ID found in claims, we cannot proceed safely
                 TempData["ErrorMessage"] = "Error: Could not identify your Barangay. Please re-login.";
                 return RedirectToAction(nameof(YouthProfiles));
            }

            // Remove Barangay_ID from ModelState as it's set programmatically
            ModelState.Remove(nameof(member.Barangay_ID));
            ModelState.Remove(nameof(member.Sitio)); // Remove navigation prop validation

            // 2. Validation: Age Logic
            // Calculate accurate age from birthday
            var calculatedAge = DateTime.Now.Year - member.Birthday.Year;
            if (member.Birthday.Date > DateTime.Now.AddYears(-calculatedAge))
                calculatedAge--;

            member.Age = calculatedAge; // Ensure age is consistent with birthday
            
            // Remove Age from ModelState to ignore form binding errors (we use calculated value)
            ModelState.Remove(nameof(member.Age));

            if (member.Age < 13 || member.Age > 21)
            {
                ModelState.AddModelError("Age", "Only youths aged 13-21 are allowed.");
            }

            if (ModelState.IsValid)
            {
                _context.YouthMembers.Add(member);
                await _context.SaveChangesAsync();

                // LOGGING
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    await _systemLogService.LogAsync(userId.Value, "Add Youth", $"Added Youth: {member.FirstName} {member.LastName}", "YouthMember", member.Member_ID);
                }

                TempData["SuccessMessage"] = "Youth member added successfully!";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            TempData["ErrorMessage"] = "Please fix the errors below.";

            // Fix: Ensure the list is still filtered by Barangay even on error
            var errorYouthList = _context.YouthMembers
                .Include(y => y.Sitio)
                .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                .ToList();
                
            // Repopulate Dropdown
            ViewBag.SitioList = _context.Sitios
                .Where(s => s.Barangay_ID == barangayId.Value)
                .OrderBy(s => s.Sitio_Name)
                .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = s.Sitio_ID.ToString(), Text = s.Sitio_Name })
                .ToList();

            // Pass the invalid model back to the view to repopulate the form
            ViewBag.NewYouth = member;
            ViewBag.ShowAddModal = true;

            return View("~/Views/BarangaySk/YouthProfiles.cshtml", errorYouthList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(YouthMember member)
        {
            var existing = await _context.YouthMembers.FindAsync(member.Member_ID);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            // Update fields
            existing.FirstName = member.FirstName;
            existing.LastName = member.LastName;
            existing.Sex = member.Sex;
            existing.Sitio_ID = member.Sitio_ID; // Update ID
            existing.Birthday = member.Birthday;
            
            // Recalculate age
            var calculatedAge = DateTime.Now.Year - member.Birthday.Year;
            if (member.Birthday.Date > DateTime.Now.AddYears(-calculatedAge))
                calculatedAge--;
            existing.Age = calculatedAge;

            await _context.SaveChangesAsync();

            // LOGGING
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Update Youth", $"Updated Youth: {existing.FirstName} {existing.LastName}", "YouthMember", existing.Member_ID);
            }

            TempData["SuccessMessage"] = "Member updated successfully!";
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int Member_ID)
        {
            var member = await _context.YouthMembers.FindAsync(Member_ID);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            member.IsArchived = true;
            await _context.SaveChangesAsync();

            // LOGGING
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Archive Youth", $"Archived Youth: {member.FirstName} {member.LastName}", "YouthMember", member.Member_ID);
            }

            TempData["SuccessMessage"] = "Member archived successfully!";
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreSelected(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var members = await _context.YouthMembers.Where(m => selectedIds.Contains(m.Member_ID)).ToListAsync();
                foreach (var m in members)
                {
                    m.IsArchived = false;
                }
                await _context.SaveChangesAsync();

                // LOGGING
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    await _systemLogService.LogAsync(userId.Value, "Restore Youth", $"Restored {members.Count} Youth Members", "YouthMember", null);
                }

                TempData["SuccessMessage"] = "Selected members restored successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "No members selected.";
            }
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSitio(string SitioName)
        {
            if (string.IsNullOrWhiteSpace(SitioName))
            {
                TempData["ErrorMessage"] = "Sitio Name is required.";
                return RedirectToAction(nameof(YouthProfiles));
            }

            var barangayId = GetBarangayIdFromClaims();
            if (barangayId == null)
            {
                TempData["ErrorMessage"] = "User is not assigned to a Barangay.";
                return RedirectToAction(nameof(YouthProfiles));
            }

            var exists = await _context.Sitios.AnyAsync(s => s.Barangay_ID == barangayId && s.Sitio_Name == SitioName);
            if (exists)
            {
                TempData["ErrorMessage"] = "Sitio already exists in this Barangay.";
                return RedirectToAction(nameof(YouthProfiles));
            }

            var sitio = new Sitio
            {
                Sitio_Name = SitioName,
                Barangay_ID = barangayId.Value
            };

            _context.Sitios.Add(sitio);
            await _context.SaveChangesAsync();

            // LOGGING
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _systemLogService.LogAsync(userId.Value, "Add Sitio", $"Added Sitio: {SitioName}", "Sitio", sitio.Sitio_ID);
            }

            TempData["SuccessMessage"] = "Sitio added successfully!";
            return RedirectToAction(nameof(YouthProfiles));
        }
    }
}
