namespace FFMpegUI.Models
{
    public class FFMpegCreateProcessAltCommand
    {
        public IEnumerable<long> QFileServerFileIds { get; set; } = Enumerable.Empty<long>();
        public int? OverallConversionQuality { get; set; }
        public FFMpegAudioCodecType? AudioCodec { get; set; }
        public FFMpegVideoCodecType? VideoCodec { get; set; }
        public int? RescaleHorizontalWidth { get; set; }
    }
}
