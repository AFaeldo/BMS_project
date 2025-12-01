using System.Threading.Tasks;

namespace BMS_project.Services
{
    public interface ISystemLogService
    {
        Task LogAsync(int userId, string action, string remark, string? tableName = null, int? recordId = null);
    }
}
