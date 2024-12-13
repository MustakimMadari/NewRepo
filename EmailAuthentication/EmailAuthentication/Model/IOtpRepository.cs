namespace EmailAuthentication.Model
{
    public interface IOtpRepository
    {
        Task<int> SaveOtpAsync(string email, string otpCode, DateTime expiryDate);
        int VerifyOtp(string email, string otpCode, DateTime currentDate);
    }
}
