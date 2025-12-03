using BMS_project.Models;
using System.Collections.Generic;

namespace BMS_project.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBarangays { get; set; }
        public int TotalUsers { get; set; }
        public string CurrentTerm { get; set; }
        public decimal FederationFundAmount { get; set; }
        public List<KabataanTermPeriod> AllTerms { get; set; } = new List<KabataanTermPeriod>();
        public int SelectedTermId { get; set; }

        // Barangay Dashboard Stats
        public int TotalYouth { get; set; }
        public int TotalApprovedProjects { get; set; }
        public int TotalOngoingProjects { get; set; }
        public int TotalPendingProjects { get; set; }

        // Charts Data
        public string SexDistributionLabels { get; set; } // JSON
        public string SexDistributionData { get; set; } // JSON
        public string AgeDistributionLabels { get; set; } // JSON
        public string AgeDistributionData { get; set; } // JSON
        public string SitioDistributionLabels { get; set; } // JSON
        public string SitioDistributionData { get; set; } // JSON
        public string ProjectStatusData { get; set; } // JSON for Project Status Pie Chart

        // Calendar Data
        public string CalendarEvents { get; set; } // JSON List of objects { title, start, end }
    }
}