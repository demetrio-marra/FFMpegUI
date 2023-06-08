namespace FFMpegUI.Persistence.Entities
{
    public class FFMpegPersistedProcess
    {
        public int Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<FFMpegPersistedProcessItem> Items { get; set; } = new List<FFMpegPersistedProcessItem>();
        public long? SourceFilesTotalSize { get; set; }
        public long? ConvertedFilesTotalSize { get; set; }
    }
}
