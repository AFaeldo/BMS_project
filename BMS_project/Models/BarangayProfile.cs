using System.ComponentModel.DataAnnotations;

namespace BMS_project.Models
{
    public class BarangayProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Barangay { get; set; }

        [MaxLength(200)]
        public string? PostalAddress { get; set; }

        [MaxLength(50)]
        public string? Zone { get; set; }

        [MaxLength(50)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(255)]
        public string? LogoPath { get; set; } // stores image path (e.g. /images/logo123.png)
    }
}
