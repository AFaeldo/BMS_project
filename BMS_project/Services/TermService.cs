using BMS_project.Data;
using BMS_project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BMS_project.Services
{
    public class TermService : ITermService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemLogService _systemLogService;

        public TermService(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
        }

        public async Task<(bool IsSuccess, string Message)> ValidateDuration(DateTime startDate, DateTime endDate)
        {
            // Duration Validation (Approximate 3 years)
            // Allow a buffer of +/- 60 days around the 3-year mark
            var expectedDuration = TimeSpan.FromDays(365 * 3);
            var actualDuration = endDate - startDate;
            var diffDays = Math.Abs((expectedDuration - actualDuration).TotalDays);

            if (diffDays > 60)
            {
                return (false, $"The term duration must be approximately 3 years. You entered a duration of {actualDuration.TotalDays / 365.0:N1} years.");
            }
            return (true, "Valid");
        }

        public async Task<(bool IsSuccess, string Message)> CreateTermAsync(string termName, DateTime startDate, DateTime endDate, int userId, bool isActive = false)
        {
            var validation = await ValidateDuration(startDate, endDate);
            if (!validation.IsSuccess) return (false, validation.Message);

            if (isActive)
            {
                // If strictly trying to set active immediately, check for existing
                var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
                if (activeTerm != null) return (false, "Cannot set as active because there is already an active term.");
            }

            var newTerm = new KabataanTermPeriod
            {
                Term_Name = termName,
                Start_Date = startDate,
                Official_End_Date = endDate,
                IsActive = isActive
            };

            _context.KabataanTermPeriods.Add(newTerm);
            await _context.SaveChangesAsync();

            await _systemLogService.LogAsync(userId, "Add Term", $"Added New Term: {termName}", "KabataanTermPeriod", newTerm.Term_ID);

            return (true, "Term added successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> ActivateTermAsync(int termId, int userId)
        {
            var termToActivate = await _context.KabataanTermPeriods.FindAsync(termId);
            if (termToActivate == null) return (false, "Term not found.");

            if (termToActivate.IsActive) return (true, "Term is already active.");

            // Deactivate all other terms
            var activeTerms = await _context.KabataanTermPeriods.Where(t => t.IsActive).ToListAsync();
            foreach (var t in activeTerms)
            {
                t.IsActive = false;
            }

            termToActivate.IsActive = true;
            await _context.SaveChangesAsync();

            await _systemLogService.LogAsync(userId, "Activate Term", $"Activated Term: {termToActivate.Term_Name}", "KabataanTermPeriod", termId);

            return (true, "Term activated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTermAsync(int termId, string termName, DateTime startDate, DateTime endDate, int userId)
        {
            var term = await _context.KabataanTermPeriods.FindAsync(termId);
            if (term == null) return (false, "Term not found.");

            if (!term.IsActive) return (false, "Only the active term can be edited.");

            var validation = await ValidateDuration(startDate, endDate);
            if (!validation.IsSuccess) return (false, validation.Message);

            term.Term_Name = termName;
            term.Start_Date = startDate;
            term.Official_End_Date = endDate;

            await _context.SaveChangesAsync();

            await _systemLogService.LogAsync(userId, "Update Term", $"Updated Active Term: {termName}", "KabataanTermPeriod", termId);

            return (true, "Term updated successfully.");
        }
    }
}