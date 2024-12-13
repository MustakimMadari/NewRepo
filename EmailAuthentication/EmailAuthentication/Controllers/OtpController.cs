using EmailAuthentication.Model;
using Microsoft.AspNetCore.Mvc;


namespace EmailAuthentication.Controllers
{
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;

        public OtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] string email)
        {
            var result = await _otpService.GenerateOtpAsync(email);

            if ((int)result < 103)
            {
                return Ok(StatusCodeReply(result));
            }
            else
            {
                return BadRequest(StatusCodeReply(result));
            }
        }

        [HttpPost("verify")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = _otpService.VerifyOtp(request.Email, request.Otp);
            
            if ((int)result < 103)
            {
                return Ok(StatusCodeReply(result));
            }
            else
            {
                return BadRequest(StatusCodeReply(result));
            }

        }

        private string StatusCodeReply(StatusCodeEnum.Status code)
        {
            switch (code)
            {
                case StatusCodeEnum.Status.STATUS_EMAIL_OK: return "Email containing OTP has been sent successfully.";
                case StatusCodeEnum.Status.STATUS_EMAIL_FAIL: return "Email address does not exist or sending to the email has failed.";
                case StatusCodeEnum.Status.STATUS_EMAIL_INVALID: return "Email address is invalid.";
                case StatusCodeEnum.Status.STATUS_OTP_OK: return "OTP is valid and checked";
                case StatusCodeEnum.Status.STATUS_OTP_FAIL: return "OTP is wrong after 10 tries";
                case StatusCodeEnum.Status.STATUS_OTP_TIMEOUT: return "timeout after 1 min";
            }
            return "Server Error.";
        }
    }

}
