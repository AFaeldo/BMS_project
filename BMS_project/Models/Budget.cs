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

        public decimal budget { get; set; }   
        public decimal disbursed { get; set; }
        public decimal balance { get; set; }

        public bool InitialBalanceSet { get; set; } = false;
        
        public decimal InitialBalance { get; set; } = 0;

        public int? Term_ID { get; set; }

        public virtual Barangay Barangay { get; set; }
    }
}
