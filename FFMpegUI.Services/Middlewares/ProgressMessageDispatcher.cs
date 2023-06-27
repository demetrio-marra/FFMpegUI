using FFMpegUI.Infrastructure;
using FFMpegUI.Messages;
using Newtonsoft.Json;

namespace FFMpegUI.Services.Middlewares
{
    public class ProgressMessageDispatcher : IProgressMessagesDispatcher
    {
        private readonly HttpClient httpClient;


        public ProgressMessageDispatcher(IHttpClientFactory factory)
        {
            httpClient = factory.CreateClient(Constants.FFMpegUIMvcProgressMessagesEndpointClientName);
        }


        async Task IProgressMessagesDispatcher.DispatchProcessItemProgress(FFMpegProcessItemMessage message)
        {
            var progressMessageStringContent = new StringContent(JsonConvert.SerializeObject(message));
            await httpClient.PostAsync("ProcessItemProgress", progressMessageStringContent);
        }
    }
}
