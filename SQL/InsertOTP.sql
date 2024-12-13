CREATE OR ALTER PROCEDURE InsertOTP
    @Email NVARCHAR(255),
    @Otp NVARCHAR(6),
    @Expiry DATETIME
AS
BEGIN
    INSERT INTO OTPTable (Email, Otp, Expiry)
    VALUES (@Email, @Otp, @Expiry)
END