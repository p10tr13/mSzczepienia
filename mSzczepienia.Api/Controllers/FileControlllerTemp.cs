using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public FileController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file has been sent");

            try
            {
                using var stream = file.OpenReadStream();
                await _blobService.UploadFileAsync(file.FileName, stream);
                return Ok(new { message = $"Success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}