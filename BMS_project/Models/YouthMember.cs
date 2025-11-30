using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("youth_member")]
    public class YouthMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Member_ID")]
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
        [Column("Sitio")]
        public string Sitio { get; set; }

        [Required, DataType(DataType.Date)]
        [Column("Birthday")]
        public DateTime Birthday { get; set; }

        [Column("Barangay_ID")]
        public int? Barangay_ID { get; set; }

        [Column("IsArchived")]
        public bool IsArchived { get; set; } = false;
    }
}