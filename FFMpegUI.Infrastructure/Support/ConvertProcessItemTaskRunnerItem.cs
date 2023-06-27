using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.Support
{
    public class ConvertProcessItemTaskRunnerItem
    {
        public long QFileServerFileId { get; set; }
        public string CallbackUrl { get; set; }
        public FFMpegConvertParameters ConvertParameters { get; set; }
    }
}
