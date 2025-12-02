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

        public async Task<(bool IsSuccess, string Message)> CanCreateNewTermAsync(DateTime startDate, DateTime endDate)
        {
            // 1. Duration Validation (Strictly 3 years)
            // We accept exactly 3 years (Same date 3 years later) or 3 years minus 1 day.
            var threeYearsLater = startDate.AddYears(3);
            bool isExactThreeYears = endDate.Date == threeYearsLater.Date;
            bool isThreeYearsMinusOneDay = endDate.Date == threeYearsLater.AddDays(-1).Date;

            if (!isExactThreeYears && !isThreeYearsMinusOneDay)
            {
                return (false, "The term duration must be strictly 3 years.");
            }

            // 2. Overlap / Active Check
            // Requirement: "Prevent creating a new term if the current term has not yet reached its Official_End_Date or if IsActive is still true."

            // Check if there is any Active term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm != null)
            {
                return (false, "Cannot create a new term because there is still an active term.");
            }

            // Check if the most recent term has reached its end date
            var latestTerm = await _context.KabataanTermPeriods
                                           .OrderByDescending(t => t.Official_End_Date)
                                           .FirstOrDefaultAsync();

            if (latestTerm != null && DateTime.Now < latestTerm.Official_End_Date)
            {
                 return (false, $"Cannot create a new term. The current term officially ends on {latestTerm.Official_End_Date:MMM dd, yyyy}.");
            }

            return (true, "Valid");
        }

        public async Task<(bool IsSuccess, string Message)> CreateTermAsync(string termName, DateTime startDate, DateTime endDate, int userId)
        {
            // Validate first
            var validation = await CanCreateNewTermAsync(startDate, endDate);
            if (!validation.IsSuccess)
            {
                return (false, validation.Message);
            }

            // Create new term
            var newTerm = new KabataanTermPeriod
            {
                Term_Name = termName,
                Start_Date = startDate,
                Official_End_Date = endDate,
                IsActive = true
            };

            _context.KabataanTermPeriods.Add(newTerm);
            await _context.SaveChangesAsync();

            // Log the action
            await _systemLogService.LogAsync(userId, "Set Term", $"Set New Term: {termName}", "KabataanTermPeriod", newTerm.Term_ID);

            return (true, "New Term Period set successfully.");
        }
    }
}
