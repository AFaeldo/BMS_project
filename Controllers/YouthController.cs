using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.Models;
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

        // ? Show the YouthProfiles page
        [HttpGet]
        public IActionResult YouthProfiles()
        {
            var youthList = _context.YouthMembers.ToList();
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
        }

        // New: return the table as a partial (AJAX / client-side load)
        [HttpGet]
        public IActionResult GetYouthTable()
        {
            var youthList = _context.YouthMembers.ToList();
            return PartialView("~/Views/BarangaySk/_YouthTable.cshtml", youthList);
        }

        // ? Add a new youth member (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(YouthMember member)
        {
            if (ModelState.IsValid)
            {
                // Calculate age
                member.Age = DateTime.Now.Year - member.Birthday.Year;
                if (member.Birthday.Date > DateTime.Now.AddYears(-member.Age))
                    member.Age--;

                _context.YouthMembers.Add(member);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Youth member added successfully!";
                return RedirectToAction(nameof(YouthProfiles));
            }

            TempData["ErrorMessage"] = "Please fill in all required fields.";
            var youthList = _context.YouthMembers.ToList();
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", youthList);
        }
    }
}