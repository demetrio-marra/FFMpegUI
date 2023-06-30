using FFMpegUI.Models;

namespace FFMpegUI.Services
{
    public interface IFFMpegConvertingService
    {
        [Obsolete("Use ConvertEmittingMessages", false)]
        Task<FFMpegConvertedFileDTO> Convert(long qFileServerFileId, int processItemId, int processId, FFMpegConvertParameters parameters);
        Task<FFMpegConvertedFileDTO> ConvertEmittingMessages(long qFileServerFileId, int processItemId, int processId, FFMpegConvertParameters parameters);
    }
}
