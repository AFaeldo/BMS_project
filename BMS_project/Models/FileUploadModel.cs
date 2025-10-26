namespace BMS_project.Models
{
    public class FileUploadModel
    {
        public int File_ID { get; set; }
        public string File_Name { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;
        public string? File_Category { get; set; }
        public int? Project_ID { get; set; }
        public int? User_ID { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
