using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("kabataan_service_record")]
    public class KabataanServiceRecord
    {
        [Key]
        [Column("Record_ID")]
        public int Record_ID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Column("Term_ID")]
        public int? Term_ID { get; set; }

        [ForeignKey("Term_ID")]
        public KabataanTermPeriod KabataanTermPeriod { get; set; }

        [Column("Role_ID")]
        public int Role_ID { get; set; }

        [ForeignKey("Role_ID")]
        public Role Role { get; set; }

        [Column("Status")] 
        public string Status { get; set; } = "Active";

        [Column("Actual_End_Date")]
        public DateTime? Actual_End_Date { get; set; }
    }
}
