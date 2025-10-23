using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}
