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

        public List<BarangayExpense> BarangayExpenseList { get; set; } = new List<BarangayExpense>();
        public string ChartLabelsJson { get; set; }
        public string ChartDataJson { get; set; }
    }

    public class BarangayExpense
    {
        public string BarangayName { get; set; }
        public decimal TotalDisbursed { get; set; }
    }
}
