using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.ViewModels;
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
            var model = new YouthProfileViewModel
            {
                YouthList = youthList
            };
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", model);
        }

        // ? Add a new youth member (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(YouthProfileViewModel vm)
        {
            if (vm == null) vm = new YouthProfileViewModel();

            if (ModelState.IsValid)
            {
                var member = vm.NewMember;
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
            vm.YouthList = _context.YouthMembers.ToList();
            return View("~/Views/BarangaySk/YouthProfiles.cshtml", vm);
        }
    }
}