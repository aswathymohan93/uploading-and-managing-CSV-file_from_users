using CSV_file_management.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSV_file_management.Controllers
{
    /// <summary>
    /// Controller to handle uploading and processing of suppressed user CSV files.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SuppressedUsersController : ControllerBase
    {
        private readonly ISuppressedUserService _service;

        public SuppressedUsersController(ISuppressedUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Uploads a suppressed user CSV file and processes it.
        /// </summary>
        /// <param name="file">The CSV file uploaded via multipart/form-data.</param>
        /// <returns>Returns success or error message depending on processing result.</returns>
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
