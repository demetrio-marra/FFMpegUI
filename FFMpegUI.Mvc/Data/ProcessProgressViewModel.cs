namespace FFMpegUI.Mvc.Data
{
    public class ProcessProgressViewModel
    {
        public string? ProcessId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ConvertedFilesTotalSize { get; set; }
        public string? AllFilesTotalSize { get; set; }
        public string? ProgressMessage { get; set; }
    }
}
