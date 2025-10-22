using System.Data;

namespace BaranggayManagementSystem.Models
{
    public class User
    {
        public int User_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Foreign Keys
        public int Role_ID { get; set; }
        public Role Role { get; set; }
    }
}
