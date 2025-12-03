using BMS_project.Models;
using System.Collections.Generic;

namespace BMS_project.ViewModels
{
    public class FederationDashboardViewModel
    {
        public int TotalApprovedProjects { get; set; }
        public int TotalPendingProjects { get; set; }
        public decimal TotalFederationBudget { get; set; }
        public decimal TotalDisbursed { get; set; } // Allocated to Barangays
        public decimal TotalRemainingBalance { get; set; } // Federation Remaining

        // Term Filter
        public List<KabataanTermPeriod> AllTerms { get; set; } = new List<KabataanTermPeriod>();
        public int SelectedTermId { get; set; }
        public string CurrentTermName { get; set; }

        // Chart Data: Monthly Expenses (Federation aggregate)
        public string MonthlyExpensesLabelsJson { get; set; }
        public string MonthlyExpensesDataJson { get; set; }

        // Chart Data: Barangay Budget Allocation
        public string BarangayBudgetLabelsJson { get; set; }
        public string BarangayBudgetDataJson { get; set; }
    }
}
