using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("Role_ID")]
        public int Role_ID { get; set; }

        [Column("Role_Name")]
        public string Role_Name { get; set; }

        // Navigation property
        public ICollection<Login> Logins { get; set; }
    }
}
