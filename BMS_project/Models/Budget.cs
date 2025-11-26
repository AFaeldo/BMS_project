using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("budget")]
    public class Budget
    {
        [Key]
        public int Budget_ID { get; set; }

        public int Barangay_ID { get; set; }

        // DB columns from your ERD: budget, disbursed, balance
        public decimal budget { get; set; }   // Allotment
        public decimal disbursed { get; set; }
        public decimal balance { get; set; }

        // Navigation property
        public virtual Barangay Barangay { get; set; }
    }
}
