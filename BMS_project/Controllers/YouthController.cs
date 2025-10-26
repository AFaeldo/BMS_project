using Microsoft.AspNetCore.Mvc;
using BMS_project.Data;
using BMS_project.Models;
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

        // LIST: All youth
        public IActionResult Index()
        {
            var members = _context.YouthMembers.ToList();
            return View("~/Views/BarangaySK/YouthProfiles.cshtml", members);
        }


        // GET: Add Youth
        public IActionResult Add()
        {
            return View();
        }

        // POST: Add Youth
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(YouthMember member)
        {
            if (ModelState.IsValid)
            {
                // Auto-calc Age from Birthday
                member.Age = DateTime.Now.Year - member.Birthday.Year;
                if (member.Birthday.Date > DateTime.Now.AddYears(-member.Age))
                    member.Age--;

                _context.YouthMembers.Add(member);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Youth member added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Edit Youth
        public IActionResult Edit(int id)
        {
            var member = _context.YouthMembers.FirstOrDefault(m => m.Member_ID == id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Youth member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // POST: Edit Youth
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(YouthMember member)
        {
            if (ModelState.IsValid)
            {
                // Recalculate Age
                member.Age = DateTime.Now.Year - member.Birthday.Year;
                if (member.Birthday.Date > DateTime.Now.AddYears(-member.Age))
                    member.Age--;

                _context.YouthMembers.Update(member);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Youth member updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Delete Youth
        public IActionResult Delete(int id)
        {
            var member = _context.YouthMembers.FirstOrDefault(m => m.Member_ID == id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Youth member not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.YouthMembers.Remove(member);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Youth member deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
