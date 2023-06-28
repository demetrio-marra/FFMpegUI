using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.DTOs
{
    public class FFMpegConvertItemDTO
    {
        public int ProcessId { get; set; }
        public long QFileServerFileId { get; set; }
        public int ProcessItemId { get; set; }
        public FFMpegConvertParameters Parameters { get; set; }

    }
}
