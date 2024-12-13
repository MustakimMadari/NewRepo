namespace EmailAuthentication.Model
{
    public class StatusCodeEnum
    {
        public enum Status
        {
            STATUS_EMAIL_OK = 101,
            STATUS_OTP_OK = 102,
            STATUS_EMAIL_INVALID = 103,
            STATUS_EMAIL_FAIL = 104,
            STATUS_OTP_FAIL = 105,
            STATUS_OTP_TIMEOUT = 106
        }
    }
}
