using AutoMapper;
using FFMpegUI.Infrastructure.Services;
using FFMpegUI.Models;
using FFMpegUI.Mvc.Data;
using FFMpegUI.Mvc.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FFMpegUI.Mvc.Services
{
    public class PresentationUpdater : IPresentationUpdater
    {
        private readonly IHubContext<ReportProgressHub> hubContext;
        private readonly IMapper mapper;
        private readonly ILogger<PresentationUpdater> logger;


        public PresentationUpdater(
            IHubContext<ReportProgressHub> hubContext,
            IMapper mapper,
            ILogger<PresentationUpdater> logger
            )
        {
            this.hubContext = hubContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        async Task IPresentationUpdater.UpdateProcess(FFMpegProcessStatusNotification message)
        {
            var viewModel = mapper.Map<ProcessProgressViewModel>(message);
            await hubContext.Clients.All.SendAsync("OnProcessMessage", viewModel);
        }


        async Task IPresentationUpdater.UpdateProcessItem(FFMpegProcessItemStatusNotification message)
        {
            var viewModel = mapper.Map<ProcessItemProgressViewModel>(message);
            await hubContext.Clients.All.SendAsync("OnProcessItemMessage", viewModel);
        }
    }
}
