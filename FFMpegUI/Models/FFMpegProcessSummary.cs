namespace FFMpegUI.Models
{
    public class FFMpegProcessSummary
    {
        public int? ProcessId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int FilesCount { get; set; }
        public long TotalFilesSize { get; set; }
        public string? StatusMessage { get; set; }
    }
}
