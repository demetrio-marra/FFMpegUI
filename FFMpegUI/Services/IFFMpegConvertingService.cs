using FFMpegUI.Models;

namespace FFMpegUI.Services
{
    public interface IFFMpegConvertingService
    {
        Task<FFMpegConvertedFileDTO> ConvertEmittingMessages(long qFileServerFileId, int processItemId, int processId, FFMpegConvertParameters parameters);
    }
}
