using System.ComponentModel.DataAnnotations;
using BMS_project.Models;

namespace BMS_project.Models
{
    public class Budget
    {
        [Key]
        public int Budget_ID { get; set; }

        public int Barangay_ID { get; set; }
        public int User_ID { get; set; }      // who encoded (Federation President)

        // DB columns from your ERD: budget, disbursed, balance
        public decimal budget { get; set; }   // Allotment
        public decimal disbursed { get; set; }
        public decimal balance { get; set; }

        // Navigation properties
        public virtual Barangay Barangay { get; set; }
        public virtual User User { get; set; }
    }
}
