using FFMpegUI.Models;

namespace FFMpegUI.Services
{
    public interface IFFMpegConvertingService
    {
        Task<FFMpegConvertedFileDTO> Convert(long qFileServerFileId, FFMpegConvertParameters parameters);
    }
}
