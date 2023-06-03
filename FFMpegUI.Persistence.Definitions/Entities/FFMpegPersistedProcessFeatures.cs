using FFMpegUI.Models;

namespace FFMpegUI.Persistence.Entities
{
    public class FFMpegPersistedProcessFeatures
    {
        public int ProcessId { get; set; }
        public int? OverallConversionQuality { get; set; }
        public FFMpegAudioCodecType? AudioCodec { get; set; }
        public FFMpegVideoCodecType? VideoCodec { get; set; }
        public int? RescaleHorizontalWidth { get; set; }
    }
}
