using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("announcement")]
    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Announcement_ID")]
        public int Announcement_ID { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User? User { get; set; }

        [Required]
        [Column("Title")]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [Column("Message")]
        public string Message { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("Date_Created")]
        public DateTime Date_Created { get; set; } = DateTime.Now;
    }
}
