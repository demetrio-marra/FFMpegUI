using FFMpegUI.Models;
using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FFMpegUI.Mvc.Pages
{
    public class NuovoProcesso : PageModel
    {
        private readonly ILogger<ProcessiModel> _logger;
        private readonly IFFMpegManagementService service;

        [BindProperty]
        public int? OverallConversionQuality { get; set; }

        [BindProperty]
        public FFMpegAudioCodecType? AudioCodec { get; set; }

        [BindProperty]
        public FFMpegVideoCodecType? VideoCodec { get; set; }

        [BindProperty]
        public int? RescaleHorizontalWidth { get; set; }

        public SelectList AudioCodecList { get; set; }
        public SelectList VideoCodecList { get; set; }

        public NuovoProcesso(ILogger<ProcessiModel> logger,
            IFFMpegManagementService service)
        {
            _logger = logger;
            this.service = service;

            // Initialize SelectList for AudioCodec and VideoCodec
            AudioCodecList = new SelectList(Enum.GetValues(typeof(FFMpegAudioCodecType)));
            VideoCodecList = new SelectList(Enum.GetValues(typeof(FFMpegVideoCodecType)));
        }

        public void OnGet()
        {
            // defaults
            OverallConversionQuality = 23; //default
            AudioCodec = FFMpegAudioCodecType.AAC;
            VideoCodec = FFMpegVideoCodecType.H264;
            RescaleHorizontalWidth = 1280;
        }

        public async Task<IActionResult> OnPostAsync(IFormFileCollection files)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var command = new FFMpegCreateProcessCommand();
                    var fileList = files.Select(f => new FFMpegCreateProcessCommand.FFMpegCreateProcessCommandItem
                    {
                        UpcomingFileName = f.FileName,
                        UpcomingStream = f.OpenReadStream()
                    }).ToList();

                    command.Files = fileList;

                    // Set the properties on the command
                    command.OverallConversionQuality = OverallConversionQuality;
                    command.AudioCodec = AudioCodec;
                    command.VideoCodec = VideoCodec;
                    command.RescaleHorizontalWidth = RescaleHorizontalWidth;

                    await service.CreateProcess(command);
                }
            }
            return Page();
        }
    }
}
