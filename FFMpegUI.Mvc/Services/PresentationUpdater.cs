﻿using AutoMapper;
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


        public PresentationUpdater(
            IHubContext<ReportProgressHub> hubContext,
            IMapper mapper
            )
        {
            this.hubContext = hubContext;
            this.mapper = mapper;
        }

        async Task IPresentationUpdater.UpdateProcess(FFMpegProcessStatusNotification message)
        {
            throw new NotImplementedException();
        }


        async Task IPresentationUpdater.UpdateProcessItem(FFMpegProcessItemStatusNotification message)
        {
            var viewModel = mapper.Map<ProcessItemProgressViewModel>(message);
            await hubContext.Clients.All.SendAsync("OnProcessItemMessage", viewModel);
        }
    }
}
