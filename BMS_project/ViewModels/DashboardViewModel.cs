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
    }
}