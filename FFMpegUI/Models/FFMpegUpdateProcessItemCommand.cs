namespace FFMpegUI.Models
{
    public class FFMpegUpdateProcessItemCommand
    {
        public int ProcessItemId { get; set; }
        public long? ConvertedFileId { get; set; }
        public string? ConvertedFileName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Successfull { get; set; }
        public string? StatusMessage { get; set; }
        public long? ConvertedFileSize { get; set; }
    }
}
