/* First-class catalog metadata for public and custom yeast records. */
SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.Yeast', N'Country') IS NULL
BEGIN
    ALTER TABLE dbo.Yeast ADD Country varchar(100) NULL;
END;

IF COL_LENGTH(N'dbo.Yeast', N'WebsiteUrl') IS NULL
BEGIN
    ALTER TABLE dbo.Yeast ADD WebsiteUrl varchar(500) NULL;
END;
