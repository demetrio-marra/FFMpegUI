using FFMpegUI.Models;

namespace FFMpegUI.Services.Middlewares
{
    public interface IFFMpegUIApiService
    {
        Task<FFMpegConvertedFileDTO> Convert(long qFileServerId, FFMpegConvertParameters parameters);
        Task ConvertEmittingMessages(long qFileServerId, int processItemId, FFMpegConvertParameters parameters);
    }
}