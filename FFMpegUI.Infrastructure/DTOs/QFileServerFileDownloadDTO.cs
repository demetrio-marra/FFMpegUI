namespace QFileServer.Mvc.Models
{
    public class QFileServerFileDownloadDTO
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream FileStream { get; set; }
    }
}
