using FFMpegUI.Models;
using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FFMpegUI.Mvc.Pages
{
    public class DettaglioProcessoModel : PageModel
    {
        private readonly ILogger<DettaglioProcessoModel> _logger;
        private readonly IFFMpegManagementService service;

        public IEnumerable<FFMpegProcessItem> ProcessItems { get; set; } = Enumerable.Empty<FFMpegProcessItem>();
        
        public int ProcessId { get; set; }
  
        public int? OverallConversionQuality { get; set; }

        public FFMpegAudioCodecType? AudioCodec { get; set; }

        public FFMpegVideoCodecType? VideoCodec { get; set; }

        public int? RescaleHorizontalWidth { get; set; }
        public long? SourceFilesTotalSize { get; set; }
        public long? ConvertedFilesTotalSize { get; set; }


        public DettaglioProcessoModel(ILogger<DettaglioProcessoModel> logger,
            IFFMpegManagementService service)
        {
            _logger = logger;
            this.service = service;
        }


        public async Task OnGet(int id)
        {
            var process = await service.GetProcessDetails(id);

            ProcessItems = process.Items;
            ProcessId = process.ProcessId ?? 0;
            OverallConversionQuality = process.OverallConversionQuality;
            AudioCodec = process.AudioCodec;
            VideoCodec = process.VideoCodec;
            RescaleHorizontalWidth = process.RescaleHorizontalWidth;
            SourceFilesTotalSize = process.SourceFilesTotalSize;
            ConvertedFilesTotalSize = process.ConvertedFilesTotalSize;

            ViewData["Title"] = $"Process {id} detail";
        }


        public async Task<IActionResult> OnGetDownload(long id)
        {
            var file = await service.GetFileForDownload(id);
            return File(file.FileStream, file.ContentType, file.FileName);
        }
    }
}
