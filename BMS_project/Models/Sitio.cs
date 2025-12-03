using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("sitio")]
    public class Sitio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sitio_id")]
        public int Sitio_ID { get; set; }

        [Required]
        [Column("Name")]
        [StringLength(255)]
        public string Sitio_Name { get; set; }

        [Column("Barangay_id")]
        public int Barangay_ID { get; set; }

        [ForeignKey("Barangay_ID")]
        public Barangay Barangay { get; set; }
    }
}
