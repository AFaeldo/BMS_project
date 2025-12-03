using System.Collections.Generic;

namespace BMS_project.ViewModels
{
    public class FederationDashboardViewModel
    {
        public int TotalApprovedProjects { get; set; }
        public int TotalPendingProjects { get; set; }
        public decimal TotalFederationBudget { get; set; }
        public decimal TotalDisbursed { get; set; }
        public decimal TotalRemainingBalance { get; set; }

        // Chart Data: Monthly Expenses
        public string MonthlyExpensesLabelsJson { get; set; } // e.g. ["Jan", "Feb", "Mar", ...]
        public string MonthlyExpensesDataJson { get; set; }   // e.g. [5000, 12000, 8500, ...]
    }
}
