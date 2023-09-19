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
