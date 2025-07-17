using CSV_file_management.Models;

using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace CSV_file_management.Repositry
{
    public class SuppressedUserRepository
    {
        private readonly string _connectionString;

        public SuppressedUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CSVAppCon");
        }

        public async Task<int> InsertSuppressedFileAsync(string fileName, string filePath, DateTime uploadedOn)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_InsertSuppressedFile", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FilePath", filePath);
            cmd.Parameters.AddWithValue("@UploadedOn", uploadedOn);

            var fileIdParam = new SqlParameter("@FileId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(fileIdParam);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)fileIdParam.Value;
        }

        public async Task InsertSuppressedUserAsync(SuppressedUserDto user, int fileId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_InsertSuppressedUser", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@NPI", user.NPI);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@FileId", fileId);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}