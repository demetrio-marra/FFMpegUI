namespace FFMpegUI.Models
{
    public class FFMpegProcessStatusNotification
    {
        public int ProcessId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Successfull { get; set; }
        public long? ConvertedFilesTotalSize { get; set; }
        public string? ProgressMessage { get; set; }
        public long? AllFilesTotalSize { get; set; }
        public int? AllFilesCount { get; set; }
    }
}
