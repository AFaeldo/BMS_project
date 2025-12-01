using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models.Dto
{
    [Table("User")]
    public class UserListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int? BarangayId { get; set; }
        public string BarangayName { get; set; }
        public int? RoleId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Term { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
