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
                    .Where(y => y.Barangay_ID == barangayId.Value)
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
                return RedirectToAction(nameof(YouthProfiles));
            }

            TempData["ErrorMessage"] = "Please fix the errors below.";

            // Fix: Ensure the list is still filtered by Barangay even on error
            var errorYouthList = _context.YouthMembers
                .Where(y => y.Barangay_ID == barangayId.Value)
                .ToList();

            // Pass the invalid model back to the view to repopulate the form
            ViewBag.NewYouth = member;
            ViewBag.ShowAddModal = true;

            return View("~/Views/BarangaySk/YouthProfiles.cshtml", errorYouthList);
        }
    }
}
