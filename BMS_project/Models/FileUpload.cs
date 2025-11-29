using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("file_upload")]
    public class FileUpload
    {
        [Key]
        [Column("File_ID")]
        public int File_ID { get; set; }

        [Required]
        [Column("File_Name")]
        [StringLength(255)]
        public string File_Name { get; set; }

        [Required]
        [Column("File")]
        public byte[] File { get; set; } // Maps to longblob

        [Column("File_Category")]
        [StringLength(100)]
        public string File_Category { get; set; }

        [Column("Project_ID")]
        public int? Project_ID { get; set; }

        [ForeignKey("Project_ID")]
        public Project Project { get; set; }

        [Column("User_ID")]
        public int? User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Column("Timestamp")]
        public DateTime? Timestamp { get; set; } = DateTime.Now;
    }
}
