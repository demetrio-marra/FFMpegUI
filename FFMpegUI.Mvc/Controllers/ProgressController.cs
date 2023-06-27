using FFMpegUI.Messages;
using FFMpegUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FFMpegUI.Mvc.Controllers
{
    public class ProgressController : Controller
    {
        private readonly IFFMpegManagementService managementService;


        public ProgressController(IFFMpegManagementService managementService)
        {
            this.managementService = managementService;
        }


        [HttpPost("progress/processitemprogress")]
        public async Task<IActionResult> ProcessItemProgress([FromBody] FFMpegProcessItemMessage message)
        {
            await managementService.ElaborateProcessItemProgressMessage(message);

            return new OkResult();
        }
    }
}
