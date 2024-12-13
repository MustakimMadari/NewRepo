using System.Data;
using Microsoft.Data.SqlClient;

namespace EmailAuthentication.Model
{
    public class OtpRepository : IOtpRepository
    {
        private readonly string _connectionString;

        public OtpRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> SaveOtpAsync(string email, string otpCode, DateTime expiryDate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("EXEC InsertOTP @Email, @Otp, @Expiry", connection);
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });
                    command.Parameters.Add(new SqlParameter("@Otp", SqlDbType.NVarChar) { Value = otpCode });
                    command.Parameters.Add(new SqlParameter("@Expiry", SqlDbType.DateTime) { Value = expiryDate });

                    return await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred: {exception.Message}");
                return -1;
            }
        }

        public int VerifyOtp(string email, string otpCode, DateTime currentDate)
        {
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("EXEC ValidateOTP @Email, @Otp, @CurrentTime", connection);
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });
                command.Parameters.Add(new SqlParameter("@Otp", SqlDbType.NVarChar) { Value = otpCode });
                command.Parameters.Add(new SqlParameter("@CurrentTime", SqlDbType.DateTime) { Value = currentDate });

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(reader.GetOrdinal("result"));
                        }
                    }
                }
                return result;
            }
        }
    }
}
