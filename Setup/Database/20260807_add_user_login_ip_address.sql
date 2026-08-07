IF COL_LENGTH(N'dbo.UserLogin', N'IPAddress') IS NULL
BEGIN
    ALTER TABLE dbo.UserLogin ADD IPAddress nvarchar(64) NULL;
END
