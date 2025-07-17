using CSV_file_management.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSV_file_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppressedUsersController : ControllerBase
    {
        private readonly ISuppressedUserService _service;

        public SuppressedUsersController(ISuppressedUserService service)
        {
            _service = service;
        }

        [HttpPost("UploadCSV")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSuppressedUsers(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File size exceeds 10MB.");

            var result = await _service.ProcessCsvAndSaveAsync(file);

            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


    }
}
