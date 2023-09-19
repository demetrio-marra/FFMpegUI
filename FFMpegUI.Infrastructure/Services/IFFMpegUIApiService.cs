using FFMpegUI.Models;

namespace FFMpegUI.Services.Middlewares
{
    public interface IFFMpegUIApiService
    {
        Task ConvertEmittingMessages(int processId, long qFileServerId, int processItemId, FFMpegConvertParameters parameters);
    }
}