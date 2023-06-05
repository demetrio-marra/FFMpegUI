using Microsoft.AspNetCore.Http;
using QFileServer.Definitions.DTOs;
using QFileServer.Mvc.Models;

namespace FFMpegUI.Services.Middlewares
{
    public interface IQFileServerApiService
    {
        Task DeleteFile(long id);
        Task<QFileServerFileDownloadDTO> DownloadFile(long id);
        Task<string> DownloadFile(long id, string downloadDirectoryFullPath);
        Task<ODataQFileServerDTO?> ODataGetFiles(string oDataQueryString);
        Task<QFileServerDTO?> UploadFile(Stream openedStream, string fileName);
        Task<QFileServerDTO?> UploadFile(IFormFile formFile);
    }
}