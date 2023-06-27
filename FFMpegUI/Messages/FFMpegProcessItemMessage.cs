namespace FFMpegUI.Messages
{
    public class FFMpegProcessItemMessage
    {
        public int ProcessItemId { get; set; }
        public long? ConvertedFileId { get; set; }
        public string? ConvertedFileName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Successfull { get; set; }
        public string? ErrorMessage { get; set; }
        public long? ConvertedFileSize { get; set; }
        public string? ProgressMessage { get; set; }
    }
}
