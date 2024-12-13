CREATE OR ALTER PROCEDURE ValidateOTP
@Email NVARCHAR(255),
@Otp NVARCHAR(50),
@CurrentTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StoredOtp NVARCHAR(50);
    DECLARE @Expiry DATETIME;
    DECLARE @AttemptCount INT;
    DECLARE @Id INT;
	DECLARE @Result INT;

    SELECT TOP 1 @Id = Id, @StoredOtp = Otp, @Expiry = Expiry, @AttemptCount = Attempt
		FROM OTPTable
		WHERE Email = @Email;

    IF @Id IS NULL
    BEGIN
        SELECT 103 AS result;
        RETURN;
    END

    IF @CurrentTime > @Expiry
    BEGIN
        SELECT 106 AS result;
        RETURN;
    END

    IF @AttemptCount >= 10
    BEGIN
         SELECT 105 AS result;
         RETURN;
    END

    IF @Otp = @StoredOtp
    BEGIN
        DELETE FROM OTPTable WHERE Id = @Id;
		SELECT 102 AS result;
        RETURN;
    END
    ELSE
    BEGIN
        UPDATE OTPTable
			SET Attempt = @AttemptCount + 1
			WHERE Id = @Id;

        SELECT 104 AS result;
        RETURN;
    END
END
