using Microsoft.AspNetCore.Http;
using QFileServer.Definitions.DTOs;
using QFileServer.Mvc.Models;

namespace FFMpegUI.Services.Middlewares
{
    public interface IQFileServerApiService
    {
        Task DeleteFile(long id);
        Task<QFileServerFileDownloadDTO> DownloadFile(long id);
        Task<ODataQFileServerDTO?> ODataGetFiles(string oDataQueryString);
        Task<QFileServerDTO?> UploadFile(Stream openedStream, string fileName);
        Task<QFileServerDTO?> UploadFile(IFormFile formFile);
    }
}