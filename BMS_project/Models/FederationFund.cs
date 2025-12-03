using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("federation_fund")]
    public class FederationFund
    {
        [Key]
        [Column("Fund_ID")]
        public int Fund_ID { get; set; }

        [Column("Term_ID")]
        public int Term_ID { get; set; }

        [ForeignKey("Term_ID")]
        public virtual KabataanTermPeriod Term { get; set; }

        [Column("Total_Amount")]
        public decimal Total_Amount { get; set; }

        [Column("Allocated_To_Barangays")]
        public decimal Allocated_To_Barangays { get; set; } = 0;

        // Helper property to see what is left to distribute
        [NotMapped]
        public decimal Remaining_Balance => Total_Amount - Allocated_To_Barangays;
    }
}
