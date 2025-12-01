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

        [Column("Remark")]
        [Required]
        [StringLength(225)]
        public string Remark { get; set; }

        [Column("DateTime")]
        public DateTime DateTime { get; set; }
    }
}
