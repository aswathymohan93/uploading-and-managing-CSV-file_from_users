using CSV_file_management.Models;

using CSV_file_management.Repositry;
using CSV_file_management.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Layer_Management.Services.Repository
{
    public class SuppressedUserService : ISuppressedUserService
    {
        private readonly SuppressedUserRepository Repository;

        public SuppressedUserService(IConfiguration configuration)
        {
            Repository = new SuppressedUserRepository(configuration);
        }

        public async Task<(bool IsSuccess, string Message)> ProcessCsvAndSaveAsync(IFormFile file)
        {
            var users = new List<SuppressedUserDto>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var header = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(header))
                    return (false, "Missing header row");

                var columns = header.Split(',');
                if (!columns.Contains("NPI") || !columns.Contains("Email") ||
                    !columns.Contains("FirstName") || !columns.Contains("LastName"))
                    return (false, "Required columns missing");

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');

                    if (parts.Length >= 4)
                    {
                        users.Add(new SuppressedUserDto
                        {
                            NPI = parts[0],
                            Email = parts[1],
                            FirstName = parts[2],
                            LastName = parts[3]
                        });
                    }
                }
            }

            if (!users.Any())
                return (false, "CSV contains no data rows");

            // Save file to SuppressedFile folder
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SuppressedFile");
            Directory.CreateDirectory(folderPath);
            string savedPath = Path.Combine(folderPath, file.FileName);

            await using (var stream = new FileStream(savedPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                int fileId = await Repository.InsertSuppressedFileAsync(file.FileName, savedPath, DateTime.Now);

                foreach (var user in users)
                {
                    await Repository.InsertSuppressedUserAsync(user, fileId);
                }

                return (true, "CSV file processed and saved successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error saving data: {ex.Message}");
            }
        }
    }
}