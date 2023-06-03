namespace FFMpegUI.Models
{
    public class FFMpegProcessItem
    {
        public int? ProcessId { get; set; }
        public string? SourceFileFullPath { get; set; }
        public string? DestFileFullPath { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Successfull { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
