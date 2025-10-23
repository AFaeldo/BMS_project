using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace BMS_project.Models
{
    [Table("login")] // Table name in MySQL
    public class Login
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        [Required]
        public string Username { get; set; }

        [Column("password")]
        [Required]
        public string Password { get; set; }

        [Column("Role_ID")]
        public int Role_ID { get; set; }

        // Navigation property
        [ForeignKey("Role_ID")]
        public Role Role { get; set; }
    }
}
