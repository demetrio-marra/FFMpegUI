namespace FFMpegUI.Models
{
    public class FFMpegProcess
    {
        public int? ProcessId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<FFMpegProcessItem> Items { get; set; } = Enumerable.Empty<FFMpegProcessItem>();
        public int? OverallConversionQuality { get; set; }
        public FFMpegAudioCodecType? AudioCodec { get; set; }
        public FFMpegVideoCodecType? VideoCodec { get; set; }
        public int? RescaleHorizontalWidth { get; set; }
    }
}
