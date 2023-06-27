using FFMpegUI.Messages;
using Microsoft.AspNetCore.SignalR;

namespace FFMpegUI.Mvc.Hubs
{
    public class ReportProgressHub : Hub
    {
        public async Task NotifyProcessItemMessage(FFMpegProcessItemMessage message)
        {
            await Clients.All.SendAsync("OnProcessItemMessage", message);
        }
    }
}
