using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BMS_project.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public AnnouncementController(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int id)) return id;
            return null;
        }

        // GET: Announcement
        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements
                .Include(a => a.User)
                .OrderByDescending(a => a.Date_Created)
                .ToListAsync();
            return View(announcements);
        }

        // GET: Announcement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Message,IsActive")] Announcement announcement)
        {
            // Remove validation for properties set programmatically
            ModelState.Remove(nameof(announcement.User));
            ModelState.Remove(nameof(announcement.User_ID));

            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                announcement.User_ID = userId.Value;
                announcement.Date_Created = DateTime.Now;

                // If new one is Active, optional: set others to inactive? 
                // Requirement didn't specify, but typical logic implies one active modal.
                // Assuming multiple active is allowed but Login only picks the latest.

                _context.Add(announcement);
                await _context.SaveChangesAsync();

                await _systemLogService.LogAsync(userId.Value, "Create Announcement", $"Created Announcement: {announcement.Title}", "Announcement", announcement.Announcement_ID);

                TempData["SuccessMessage"] = "Announcement created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // GET: Announcement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        // POST: Announcement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Announcement_ID,Title,Message,IsActive")] Announcement announcement)
        {
            if (id != announcement.Announcement_ID) return NotFound();

            // Remove validation for properties set programmatically or unchanged
            ModelState.Remove(nameof(announcement.User));
            ModelState.Remove(nameof(announcement.User_ID));

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Announcements.AsNoTracking().FirstOrDefaultAsync(a => a.Announcement_ID == id);
                    if (existing == null) return NotFound();

                    // Preserve fields not in form
                    announcement.User_ID = existing.User_ID;
                    announcement.Date_Created = existing.Date_Created;

                    _context.Update(announcement);
                    await _context.SaveChangesAsync();

                    var userId = GetCurrentUserId();
                    if (userId.HasValue)
                    {
                        await _systemLogService.LogAsync(userId.Value, "Edit Announcement", $"Updated Announcement: {announcement.Title}", "Announcement", announcement.Announcement_ID);
                    }

                    TempData["SuccessMessage"] = "Announcement updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementExists(announcement.Announcement_ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // POST: Announcement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();

                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    await _systemLogService.LogAsync(userId.Value, "Delete Announcement", $"Deleted Announcement: {announcement.Title}", "Announcement", announcement.Announcement_ID);
                }
                
                TempData["SuccessMessage"] = "Announcement deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Announcement_ID == id);
        }
    }
}
