namespace FFMpegUI.Mvc.Data
{
    public class ProcessItemProgressViewModel
    {
        public string? ProcessItemId { get; set; }
        public string? ProgressMessage { get; set; }
        public string? ConvertedFileLink { get; set; }
        public string? ConvertedFileSize { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
