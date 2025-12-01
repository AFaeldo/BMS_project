using BMS_project.Data;
using BMS_project.Models;
using System;
using System.Threading.Tasks;

namespace BMS_project.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly ApplicationDbContext _context;

        public SystemLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int userId, string action, string remark, string? tableName = null, int? recordId = null)
        {
            var log = new SystemLog
            {
                User_ID = userId,
                Action = action,
                Remark = remark,
                Table_Name = tableName,
                Record_ID = recordId,
                DateTime = DateTime.Now
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
