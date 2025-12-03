using System;
using System.Threading.Tasks;

namespace BMS_project.Services
{
    public interface ITermService
    {
        Task<(bool IsSuccess, string Message)> ValidateDuration(DateTime startDate, DateTime endDate);
        Task<(bool IsSuccess, string Message)> CreateTermAsync(string termName, DateTime startDate, DateTime endDate, int userId, bool isActive = false);
        Task<(bool IsSuccess, string Message)> ActivateTermAsync(int termId, int userId);
        Task<(bool IsSuccess, string Message)> UpdateTermAsync(int termId, string termName, DateTime startDate, DateTime endDate, int userId);
    }
}
