using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.DTOs
{
    public class FFMpegConvertItemDTO
    {
        public long QFileServerFileId { get; set; }
        public int ProcessItemId { get; set; }
        public FFMpegConvertParameters Parameters { get; set; }

    }
}
