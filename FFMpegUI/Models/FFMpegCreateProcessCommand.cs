namespace FFMpegUI.Models
{
    public class FFMpegCreateProcessCommand
    {
        public IEnumerable<FFMpegCreateProcessCommandItem> Files { get; set; } = Enumerable.Empty<FFMpegCreateProcessCommandItem>();
        public int? OverallConversionQuality { get; set; }
        public FFMpegAudioCodecType? AudioCodec { get; set; }
        public FFMpegVideoCodecType? VideoCodec { get; set; }
        public int? RescaleHorizontalWidth { get; set; }

        public class FFMpegCreateProcessCommandItem
        {
            public Stream UpcomingStream { get; set; } = Stream.Null;
            public string UpcomingFileName { get; set; } = string.Empty;
        }
    }
}
