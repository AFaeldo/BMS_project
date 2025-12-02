using System;
using System.Threading.Tasks;

namespace BMS_project.Services
{
    public interface ITermService
    {
        Task<(bool IsSuccess, string Message)> CanCreateNewTermAsync(DateTime startDate, DateTime endDate);
        Task<(bool IsSuccess, string Message)> CreateTermAsync(string termName, DateTime startDate, DateTime endDate, int userId);
    }
}
