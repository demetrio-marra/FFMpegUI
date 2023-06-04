namespace FFMpegUI.Models
{
    public class FFMpegFileDownloadDTO
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream FileStream { get; set; }
    }
}
