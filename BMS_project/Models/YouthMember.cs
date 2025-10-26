using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("youth_member")] // 👈 maps to your existing table
    public class YouthMember
    {
        [Key]
        [Column("Member_ID")] // 👈 maps to the PK column
        public int Member_ID { get; set; }

        [Required, StringLength(50)]
        [Column("First_Name")]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        [Column("Last_Name")]
        public string LastName { get; set; }

        [Column("Age")]
        public int Age { get; set; }

        [Required]
        [Column("Gender")]
        public string Gender { get; set; }

        [Required, StringLength(100)]
        [Column("sitio")]
        public string Sitio { get; set; }

        [Required, DataType(DataType.Date)]
        [Column("Birthday")]
        public DateTime Birthday { get; set; }
    }
}
