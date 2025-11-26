using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("barangay")]
    public class Barangay
    {
        public int Barangay_ID { get; set; }//PK
        public string Barangay_Name { get; set; }
    }
}
