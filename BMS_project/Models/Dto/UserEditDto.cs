namespace BMS_project.Models.Dto
{
    public class UserEditDto
    {
        public int Id { get; set; } // 0 = create, otherwise login id
                                    // user fields
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? BarangayId { get; set; }
        public int? RoleId { get; set; }

        // login fields
        public string Username { get; set; }
        public string Password { get; set; } // empty means keep existing on update
    }
}
