/*
   Adds a parallel, versioned style reference without modifying legacy BJCP IDs.
   Safe to run more than once against an existing OpenBrew database.
*/
IF COL_LENGTH('dbo.Recipe', 'StyleCatalog') IS NULL
BEGIN
    ALTER TABLE dbo.Recipe ADD StyleCatalog varchar(32) NOT NULL
        CONSTRAINT DF_Recipe_StyleCatalog DEFAULT ('legacy-bjcp');
END
GO

IF COL_LENGTH('dbo.Recipe', 'CatalogStyleCode') IS NULL
BEGIN
    ALTER TABLE dbo.Recipe ADD CatalogStyleCode varchar(32) NULL;
END
GO

IF COL_LENGTH('dbo.Recipe', 'CatalogStyleName') IS NULL
BEGIN
    ALTER TABLE dbo.Recipe ADD CatalogStyleName varchar(160) NULL;
END
GO

UPDATE dbo.Recipe
SET StyleCatalog = 'legacy-bjcp',
    CatalogStyleCode = COALESCE(CatalogStyleCode, BjcpStyleSubCategoryId)
WHERE StyleCatalog IS NULL OR StyleCatalog = '';
GO
