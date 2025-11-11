using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace BMS_project.ViewModels
{
    public class UserFormViewModel
    {
        public int? User_ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Contact No")]
        public string Contact_No { get; set; }

        // FK selected values
        [Display(Name = "Barangay")]
        public int? Barangay_ID { get; set; }

        [Display(Name = "Role")]
        public int? Role_ID { get; set; }

        // Login fields
        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // Dropdown lists (populated in controller)
        public IEnumerable<SelectListItem> BarangayItems { get; set; }
        public IEnumerable<SelectListItem> RoleItems { get; set; }
    }
}
