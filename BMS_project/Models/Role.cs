using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BMS_project.Models;

[Table("role")]
public class Role
{
    [Key]
    [Column("Role_ID")]
    public int Role_ID { get; set; }

    [Column("Role_Name")]
    public string Role_Name { get; set; }

    public ICollection<Login> Login { get; set; }
}
