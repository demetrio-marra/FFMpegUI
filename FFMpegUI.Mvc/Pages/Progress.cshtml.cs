using FFMpegUI.Messages;
using FFMpegUI.Mvc.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FFMpegUI.Mvc.Pages
{
    public class ProgressModel : PageModel
    {
        private readonly ReportProgressHub _hubContext;

        public ProgressModel(ReportProgressHub hubContext)
        {
            _hubContext = hubContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync([FromBody] FFMpegProcessItemMessage progress)
        {
            // Dispatch progress update to the client
            await _hubContext.NotifyProcessItemMessage(progress);

            return new OkResult();
        }
    }

}
