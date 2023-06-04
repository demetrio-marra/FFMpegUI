using Microsoft.AspNetCore.Http;
using QFileServer.Definitions.DTOs;
using QFileServer.Mvc.Models;
using System.Text.Json;

namespace FFMpegUI.Services.Middlewares
{
    public class QFileServerApiService : IQFileServerApiService
    {
        static readonly string ODATA_PATH = "/odata/v1/QFileServerOData";
        static readonly string API_PATH = "/api/v1/QFileServer";
        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient httpClient;

        public QFileServerApiService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("QFileServerApiServiceClient");
        }

        public async Task<ODataQFileServerDTO?> ODataGetFiles(string oDataQueryString)
        {
            var uriString = ODATA_PATH + "?" + oDataQueryString;
            var response = await httpClient.GetAsync(new Uri(uriString, UriKind.Relative));
            response.EnsureSuccessStatusCode();

            var ret = await JsonSerializer.DeserializeAsync<ODataQFileServerDTO>(await response.Content.ReadAsStreamAsync());

            return ret;
        }

        public async Task<QFileServerDTO?> UploadFile(IFormFile formFile)
        {
            using (var rs = formFile.OpenReadStream())
            {
                var ret = await UploadFilePv(rs, formFile.FileName);
                return ret;
            }
        }

        public async Task<QFileServerDTO?> UploadFile(Stream openedStream, string fileName)
        {
            var ret = await UploadFilePv(openedStream, fileName);
            return ret;
        }

        public async Task DeleteFile(long id)
        {
            var uriString = API_PATH + "/" + id.ToString();
            var response = await httpClient.DeleteAsync(new Uri(uriString, UriKind.Relative));
            response.EnsureSuccessStatusCode();
        }

        public async Task<QFileServerFileDownloadDTO> DownloadFile(long id)
        {
            var uriString = $"{API_PATH}/download/{id}";
            var response = await httpClient.GetAsync(new Uri(uriString, UriKind.Relative));

            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers?.ContentType?.MediaType ?? "octet/stream";
            var fileName = response.Content.Headers?.ContentDisposition?.FileNameStar;

            return new QFileServerFileDownloadDTO
            {
                ContentType = contentType,
                FileName = fileName,
                FileStream = await response.Content.ReadAsStreamAsync()
            };
        }

        private async Task<QFileServerDTO?> UploadFilePv(Stream rs, string fileName)
        {
            var mpContent = new MultipartFormDataContent
            {
                { new StreamContent(rs), "File", Path.GetFileName(fileName) }
            };

            var response = await httpClient.PostAsync(new Uri(API_PATH, UriKind.Relative), mpContent);
            response.EnsureSuccessStatusCode();

            var ret = await JsonSerializer.DeserializeAsync<QFileServerDTO>(await response.Content.ReadAsStreamAsync(),
                jsonSerializerOptions);

            return ret;
        }
    }
}
