using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BMS_project.Controllers
{
    public class YouthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YouthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper to get Barangay_ID from claims
        private int? GetBarangayIdFromClaims()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Barangay_ID");
            return claim != null && int.TryParse(claim.Value, out int id) ? id : (int?)null;
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
                    .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                    .ToList();
                
                ViewBag.ArchivedYouth = _context.YouthMembers
                    .Where(y => y.Barangay_ID == barangayId.Value && y.IsArchived)
                    .ToList();

                return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
            }

            // Fallback (or if user has no barangay): return empty list
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", new List<YouthMember>());
        }

        // ✅ Add a new youth member (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(YouthMember member)
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
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Youth member added successfully!";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            TempData["ErrorMessage"] = "Please fix the errors below.";

            // Fix: Ensure the list is still filtered by Barangay even on error
            var errorYouthList = _context.YouthMembers
                .Where(y => y.Barangay_ID == barangayId.Value && !y.IsArchived)
                .ToList();

            // Pass the invalid model back to the view to repopulate the form
            ViewBag.NewYouth = member;
            ViewBag.ShowAddModal = true;

            return View("~/Views/BarangaySk/YouthProfiles.cshtml", errorYouthList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(YouthMember member)
        {
            var existing = _context.YouthMembers.Find(member.Member_ID);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            // Update fields
            existing.FirstName = member.FirstName;
            existing.LastName = member.LastName;
            existing.Gender = member.Gender;
            existing.Sitio = member.Sitio;
            existing.Birthday = member.Birthday;
            
            // Recalculate age
            var calculatedAge = DateTime.Now.Year - member.Birthday.Year;
            if (member.Birthday.Date > DateTime.Now.AddYears(-calculatedAge))
                calculatedAge--;
            existing.Age = calculatedAge;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Member updated successfully!";
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Archive(int Member_ID)
        {
            var member = _context.YouthMembers.Find(Member_ID);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("YouthProfiles", "BarangaySk");
            }

            member.IsArchived = true;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Member archived successfully!";
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestoreSelected(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var members = _context.YouthMembers.Where(m => selectedIds.Contains(m.Member_ID)).ToList();
                foreach (var m in members)
                {
                    m.IsArchived = false;
                }
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Selected members restored successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "No members selected.";
            }
            return RedirectToAction("YouthProfiles", "BarangaySk");
        }
    }
}
