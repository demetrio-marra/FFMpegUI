namespace FFMpegUI.Models
{
    public class FFMpegConvertParameters
    {
        public int? OverallConversionQuality { get; set; }
        public FFMpegAudioCodecType? AudioCodec { get; set; }
        public FFMpegVideoCodecType? VideoCodec { get; set; }
        public int? RescaleHorizontalWidth { get; set; }
    }
}
