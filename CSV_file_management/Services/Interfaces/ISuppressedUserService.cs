namespace CSV_file_management.Services.Interfaces
{
    public interface ISuppressedUserService
    {
        Task<(bool IsSuccess, string Message)> ProcessCsvAndSaveAsync(IFormFile file);
    }
}
