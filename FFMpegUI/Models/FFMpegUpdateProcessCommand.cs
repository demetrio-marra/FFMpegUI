namespace FFMpegUI.Models
{
    public class FFMpegUpdateProcessCommand
    {
        public int ProcessId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? ConvertedFilesTotalSize { get; set; }
        public bool? Successfull { get; set; }
        public string? StatusMessage { get; set; }
    }
}
