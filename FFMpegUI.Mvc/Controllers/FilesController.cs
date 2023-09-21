using FFMpegUI.Services.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace FFMpegUI.Mvc.Controllers
{
    [Route("files")]
    public class FilesController : Controller
    {
        private readonly IQFileServerApiService fileServerService;


        public FilesController(IQFileServerApiService fileServerService)
        {
            this.fileServerService = fileServerService;
        }


        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var ret = await fileServerService.UploadFile(file);
            return Json(ret);
        }
    }
}
