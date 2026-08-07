IF COL_LENGTH('dbo.NewsletterSignup', 'ConfirmationToken') IS NULL
    ALTER TABLE dbo.NewsletterSignup ADD ConfirmationToken varchar(64) NULL;

IF COL_LENGTH('dbo.NewsletterSignup', 'IsConfirmed') IS NULL
    ALTER TABLE dbo.NewsletterSignup ADD IsConfirmed bit NOT NULL CONSTRAINT DF_NewsletterSignup_IsConfirmed DEFAULT ((0));

IF COL_LENGTH('dbo.NewsletterSignup', 'IsUnsubscribed') IS NULL
    ALTER TABLE dbo.NewsletterSignup ADD IsUnsubscribed bit NOT NULL CONSTRAINT DF_NewsletterSignup_IsUnsubscribed DEFAULT ((0));

IF COL_LENGTH('dbo.NewsletterSignup', 'DateConfirmed') IS NULL
    ALTER TABLE dbo.NewsletterSignup ADD DateConfirmed datetime NULL;

IF COL_LENGTH('dbo.NewsletterSignup', 'DateUnsubscribed') IS NULL
    ALTER TABLE dbo.NewsletterSignup ADD DateUnsubscribed datetime NULL;
