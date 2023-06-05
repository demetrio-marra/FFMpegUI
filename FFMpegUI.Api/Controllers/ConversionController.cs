using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;
using FFMpegUI.Infrastructure.DTOs;
using System.Text.Json;

namespace FFMpegUI.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversionController : ControllerBase
    {       
        private readonly ILogger<ConversionController> _logger;
        private readonly IFFMpegConvertingService service;

        public ConversionController(ILogger<ConversionController> logger, IFFMpegConvertingService service)
        {
            _logger = logger;
            this.service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Index(FFMpegConvertItemDTO dto)
        {
            var convertedFile = await service.Convert(dto.QFileServerFileId, dto.Parameters);
            var ret = JsonSerializer.Serialize(convertedFile);
            return Ok(ret);
        }
    }
}
