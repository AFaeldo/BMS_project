using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BMS_project.Models
{
    [Table("login")]                // table name in MySQL
    public class Login
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("Role_ID")]         // matches real FK column
        public int Role_ID { get; set; }

        public Role Role { get; set; } // navigation property

        public int? User_ID { get; set; }             // FK -> User.User_ID
        public User User { get; set; }
    }
}
