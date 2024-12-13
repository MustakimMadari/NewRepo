using System.Net.Mail;
using System.Net;

namespace EmailAuthentication.Model
{
    public class OtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        private readonly int _otpValidityMinutes;

        public OtpService(IOtpRepository otpRepository, IConfiguration configuration)
        {
            _otpRepository = otpRepository;
            _configuration = configuration;
            _smtpClient = new SmtpClient(_configuration["SMTPSettings:Host"])
            {
                Port = int.Parse(_configuration["SMTPSettings:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["SMTPSettings:Username"],
                    _configuration["SMTPSettings:Password"]
                ),
                EnableSsl = true
            };

            _otpValidityMinutes = int.Parse(_configuration["OTPSettings:OTPValidityMinutes"]);
        }

        public async Task<StatusCodeEnum.Status> GenerateOtpAsync(string email)
        {
            try
            {

                if (!email.EndsWith(_configuration["ValidDomain"]))
                {
                    return StatusCodeEnum.Status.STATUS_EMAIL_INVALID;
                }

                var otpCode = new Random().Next(100000, 999999).ToString();
                var expiryDate = DateTime.UtcNow.AddMinutes(_otpValidityMinutes);

                int result = await _otpRepository.SaveOtpAsync(email, otpCode, expiryDate);

                if(result == -1)
                {
                    return StatusCodeEnum.Status.STATUS_EMAIL_FAIL;
                }

                if (result == 1 && Convert.ToBoolean(_configuration["IsEnable"]))
                {
                    var message = new MailMessage(_configuration["NoReplyEmail"], email)
                    {
                        Subject = "Your OTP Code",
                        Body = $"Your OTP Code is {otpCode}. It is valid for 1 minute."
                    };

                    await _smtpClient.SendMailAsync(message);
                }

                return StatusCodeEnum.Status.STATUS_EMAIL_OK;

            }
            catch(Exception exception)
            {
                Console.WriteLine($"An error occurred: {exception.Message}");
                return StatusCodeEnum.Status.STATUS_EMAIL_FAIL;
            }
        }

        public StatusCodeEnum.Status VerifyOtp(string email, string enteredOtp)
        {
            try
            {
                var CurrentDate = DateTime.UtcNow;
                int result =  _otpRepository.VerifyOtp(email, enteredOtp, CurrentDate);

                return (StatusCodeEnum.Status)result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred: {exception.Message}");
                return StatusCodeEnum.Status.STATUS_OTP_FAIL;

            }
        }
    }
}