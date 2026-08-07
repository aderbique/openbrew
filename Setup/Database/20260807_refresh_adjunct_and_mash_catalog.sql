/* Curated public defaults. Custom ingredients remain untouched. */
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Calcium Chloride')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Calcium Chloride', N'Water salt used to increase chloride and support a rounder malt profile.', 1, 1, GETDATE(), N'Water Chemistry');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Epsom Salt')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Epsom Salt', N'Magnesium sulfate for modest magnesium and sulfate adjustments.', 1, 1, GETDATE(), N'Water Chemistry');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Baking Soda')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Baking Soda', N'Sodium bicarbonate for raising mash alkalinity when appropriate.', 1, 1, GETDATE(), N'Water Chemistry');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Phosphoric Acid')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Phosphoric Acid', N'Acid for mash and sparge-water pH adjustment.', 1, 1, GETDATE(), N'Water Chemistry');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Whirlfloc Tablet')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Whirlfloc Tablet', N'Kettle fining for improved hot-break formation and beer clarity.', 1, 1, GETDATE(), N'Clarity & Stabilization');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Yeast Nutrient')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Yeast Nutrient', N'General nutrient blend to support healthy fermentation.', 1, 1, GETDATE(), N'Fermentation Aids');
IF NOT EXISTS (SELECT 1 FROM dbo.Adjunct WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Pectin Enzyme')
    INSERT dbo.Adjunct (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Pectin Enzyme', N'Enzyme used mainly with fruit additions to reduce pectin haze.', 1, 1, GETDATE(), N'Fermentation Aids');

IF NOT EXISTS (SELECT 1 FROM dbo.MashStep WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Ferulic Acid Rest')
    INSERT dbo.MashStep (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Ferulic Acid Rest', N'Optional low-temperature rest for promoting clove-like phenolics in suitable wheat beers.', 1, 1, GETDATE(), N'Mash Rests');
IF NOT EXISTS (SELECT 1 FROM dbo.MashStep WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND Name = N'Single Infusion Mash')
    INSERT dbo.MashStep (Name, Description, IsActive, IsPublic, DateCreated, Category) VALUES (N'Single Infusion Mash', N'General all-in-one mash rest for modern well-modified malt.', 1, 1, GETDATE(), N'Mash Rests');
