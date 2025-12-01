using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMS_project.Models
{
    [Table("system_log")]
    public class SystemLog
    {
        [Key]
        [Column("SysLog_id")]
        public int SysLog_id { get; set; }

        [Column("User_ID")]
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        [Column("Action")]
        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        [Column("Table_Name")]
        [StringLength(50)]
        public string? Table_Name { get; set; }

        [Column("Record_ID")]
        public int? Record_ID { get; set; }

        [Column("Remark")]
        public string? Remark { get; set; }

        [Column("DateTime")]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
