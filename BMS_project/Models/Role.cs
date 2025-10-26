using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("role")]
public class Role
{
    [Key]
    [Column("Role_ID")]
    public int Role_ID { get; set; }

    [Column("Role_Name")]
    public string Role_Name { get; set; }

    public ICollection<Login> Logins { get; set; }
}
