using Microsoft.AspNetCore.Mvc;
using FFMpegUI.Infrastructure.DTOs;
using FFMpegUI.Infrastructure.Support;

namespace FFMpegUI.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversionController : ControllerBase
    {       
        private readonly ILogger<ConversionController> _logger;
        private readonly ProcessItemBackgroundTaskQueue taskQueue;


        public ConversionController(ILogger<ConversionController> logger, ProcessItemBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            this.taskQueue = taskQueue;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        
        [HttpPost]
        [Route("ConvertEmittingMessages")]
        public async Task<IActionResult> ConvertEmittingMessages(FFMpegConvertItemDTO dto)
        {
            var added = new ConvertProcessItemTaskRunnerItem
            {
                ConvertParameters = dto.Parameters,
                QFileServerFileId = dto.QFileServerFileId,
                ProcessItemId = dto.ProcessItemId,
                ProcessId = dto.ProcessId
            };

            taskQueue.Enqueue(added);

            return await Task.FromResult(Ok());
        }
    }
}
