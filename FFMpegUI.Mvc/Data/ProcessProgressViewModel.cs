namespace FFMpegUI.Mvc.Data
{
    public class ProcessProgressViewModel
    {
        public int ProcessId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Successfull { get; set; }
        public long? ConvertedFilesTotalSize { get; set; }
        public int? AllFilesTotalSize { get; set; }
        public int? AllFilesCount { get; set; } = 0;
        public string? ProgressMessage { get; set; }
    }
}
