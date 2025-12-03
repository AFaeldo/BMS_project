using BMS_project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Column("First_Name")]
        public string First_Name { get; set; } = string.Empty;

        [Column("Last_Name")]
        public string Last_Name { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Barangay_ID")]
        public int? Barangay_ID { get; set; }         // FK

        // FK navigation
        public Barangay Barangay { get; set; }

        [Column("Role_ID")]
        public int? Role_ID { get; set; }             // FK (for convenience)

        public Role Role { get; set; }

        public Login Login { get; set; }              // optional 1:1 nav prop

        public ICollection<KabataanServiceRecord> ServiceRecords { get; set; } = new List<KabataanServiceRecord>();

        [Column("IsArchived")]
        public bool IsArchived { get; set; } = false;
    }
}
