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
        private readonly ILogger<SuppressedUsersController> _logger;

        public SuppressedUsersController(ISuppressedUserService service, ILogger<SuppressedUsersController> logger)
        {
            _service = service;
            _logger = logger;
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
            _logger.LogInformation("UploadSuppressedUsers endpoint called.");

            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload failed: No file uploaded.");
                    return BadRequest("No file uploaded.");
                }
                   

                if (file.Length > 10 * 1024 * 1024)
                {
                    _logger.LogWarning("Upload failed: File size exceeds 10MB.");
                    return BadRequest("File size exceeds 10MB.");
                }

                _logger.LogInformation("Processing file: {FileName}, Size: {Size} bytes", file.FileName, file.Length);

                var result = await _service.ProcessCsvAndSaveAsync(file);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("File processed successfully: {FileName}", file.FileName);
                    return Ok(result.Message);
                }
                else
                {
                    _logger.LogWarning("File processing failed: {Message}", result.Message);
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while uploading the file.");
                throw (new Exception("Exception" + ex.Message));
            }

        }


    }
}
