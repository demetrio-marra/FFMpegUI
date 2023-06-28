using FFMpegUI.Infrastructure.DTOs;
using FFMpegUI.Models;
using System.Text;
using System.Text.Json;

namespace FFMpegUI.Services.Middlewares
{
    public class FFMpegApiService : IFFMpegUIApiService
    {
        private readonly HttpClient httpClient;


        public FFMpegApiService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FFMpegApiServiceClient");
        }


        async Task<FFMpegConvertedFileDTO> IFFMpegUIApiService.Convert(long qFileServerId, FFMpegConvertParameters parameters)
        {
            var dto = new FFMpegConvertItemDTO
            {
                Parameters = parameters,
                QFileServerFileId = qFileServerId
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            // Send the HTTP request
            var response = await httpClient.PostAsync(new Uri("Conversion", UriKind.Relative), jsonContent);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var ret = JsonSerializer.Deserialize<FFMpegConvertedFileDTO>(result);

            return ret;
        }


        async Task IFFMpegUIApiService.ConvertEmittingMessages(int processId, long qFileServerId, int processItemId, FFMpegConvertParameters parameters)
        {
            var dto = new FFMpegConvertItemDTO
            {
                ProcessId = processId,
                Parameters = parameters,
                QFileServerFileId = qFileServerId,
                ProcessItemId = processItemId
            };

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            // Send the HTTP request
            var response = await httpClient.PostAsync(new Uri("Conversion/ConvertEmittingMessages", UriKind.Relative), jsonContent);

            response.EnsureSuccessStatusCode();
        }
    }
}
