using FFMpegUI.Models;
using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PagedList.Core;

namespace FFMpegUI.Mvc.Pages
{
    public class ProcessiModel : PageModel
    {
        public IPagedList<FFMpegProcessSummary> Processes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }


        private readonly ILogger<ProcessiModel> _logger;
        private readonly IFFMpegManagementService service;

        public ProcessiModel(ILogger<ProcessiModel> logger,
            IFFMpegManagementService service)
        {
            _logger = logger;
            this.service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            var processi = await service.GetProcessesSummary(PageNumber ?? 1, 10);
            Processes = processi;
            return Page();
        }
    }
}
