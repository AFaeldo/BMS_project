using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("kabataan_term_period")]
    public class KabataanTermPeriod
    {
        [Key]
        [Column("Term_ID")]
        public int Term_ID { get; set; }

        [Column("Term_Name")]
        [Required]
        [StringLength(100)]
        public string Term_Name { get; set; }

        [Column("Start_Date")]
        public DateTime Start_Date { get; set; }

        [Column("Official_End_Date")]
        public DateTime Official_End_Date { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = false;
    }
}
