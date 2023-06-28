using FFMpegUI.Models;

namespace FFMpegUI.Services
{
    public interface IFFMpegConvertingService
    {
        Task<FFMpegConvertedFileDTO> Convert(long qFileServerFileId, int processItemId, FFMpegConvertParameters parameters);
        Task<FFMpegConvertedFileDTO> ConvertEmittingMessages(long qFileServerFileId, int processItemId, FFMpegConvertParameters parameters);
    }
}
