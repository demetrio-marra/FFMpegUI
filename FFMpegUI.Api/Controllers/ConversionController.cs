using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;
using FFMpegUI.Infrastructure.DTOs;
using System.Text.Json;
using FFMpegUI.Infrastructure.Support;

namespace FFMpegUI.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversionController : ControllerBase
    {       
        private readonly ILogger<ConversionController> _logger;
        private readonly IFFMpegConvertingService service;
        private readonly ProcessItemBackgroundTaskQueue taskQueue;


        public ConversionController(ILogger<ConversionController> logger, IFFMpegConvertingService service, ProcessItemBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            this.service = service;
            this.taskQueue = taskQueue;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Index(FFMpegConvertItemDTO dto)
        {
            var convertedFile = await service.Convert(dto.QFileServerFileId, dto.ProcessItemId, dto.Parameters);
            var ret = JsonSerializer.Serialize(convertedFile);
            return Ok(ret);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertEmittingMessages(FFMpegConvertItemDTO dto)
        {
            var added = new ConvertProcessItemTaskRunnerItem
            {
                ConvertParameters = dto.Parameters,
                QFileServerFileId = dto.QFileServerFileId,
                ProcessItemId = dto.ProcessItemId
            };

            taskQueue.Enqueue(added);

            return await Task.FromResult(Ok());
        }
    }
}
