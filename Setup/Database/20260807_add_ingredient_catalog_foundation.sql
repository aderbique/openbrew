/*
   Backwards-compatible ingredient catalog foundation.
   Existing Hop/Fermentable/Yeast/Adjunct rows remain the recipe-facing defaults.
   Product and provenance rows are optional, additive metadata for catalog updates,
   importers, and future lot-aware recipe editing.
*/
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.IngredientSource', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IngredientSource (
        IngredientSourceId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IngredientTypeId int NOT NULL,
        IngredientId int NOT NULL,
        FieldName varchar(100) NOT NULL,
        FieldValue varchar(1000) NULL,
        SourceUrl varchar(500) NOT NULL,
        SourceName varchar(150) NOT NULL,
        RetrievedAt datetime NOT NULL,
        Confidence varchar(20) NOT NULL CONSTRAINT DF_IngredientSource_Confidence DEFAULT ('medium')
    );
    CREATE UNIQUE INDEX UX_IngredientSource_Field_Source
        ON dbo.IngredientSource (IngredientTypeId, IngredientId, FieldName, SourceUrl);
END;

IF OBJECT_ID(N'dbo.HopProduct', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HopProduct (
        HopProductId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        HopId int NOT NULL,
        SupplierName varchar(150) NOT NULL,
        ProductName varchar(200) NOT NULL,
        HarvestYear smallint NULL,
        AlphaAcid decimal(5,2) NULL,
        BetaAcid decimal(5,2) NULL,
        Form varchar(50) NULL,
        LotNumber varchar(100) NULL,
        ProductUrl varchar(500) NULL,
        RetrievedAt datetime NOT NULL CONSTRAINT DF_HopProduct_RetrievedAt DEFAULT (GETDATE()),
        IsActive bit NOT NULL CONSTRAINT DF_HopProduct_IsActive DEFAULT (1),
        CONSTRAINT FK_HopProduct_Hop FOREIGN KEY (HopId) REFERENCES dbo.Hop(HopId)
    );
    CREATE UNIQUE INDEX UX_HopProduct_Supplier_Product_Lot
        ON dbo.HopProduct (HopId, SupplierName, ProductName, LotNumber)
        WHERE LotNumber IS NOT NULL;
    CREATE INDEX IX_HopProduct_Hop_Supplier ON dbo.HopProduct (HopId, SupplierName);
END;

IF OBJECT_ID(N'dbo.MaltType', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MaltType (
        MaltTypeId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name varchar(150) NOT NULL,
        Description varchar(1000) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_MaltType_IsActive DEFAULT (1),
        CONSTRAINT UX_MaltType_Name UNIQUE (Name)
    );
END;

IF OBJECT_ID(N'dbo.MaltProduct', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MaltProduct (
        MaltProductId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MaltTypeId int NOT NULL,
        FermentableId int NULL,
        ManufacturerName varchar(150) NOT NULL,
        ProductName varchar(200) NOT NULL,
        Country varchar(100) NULL,
        Ppg int NULL,
        Lovibond decimal(6,2) NULL,
        ProductUrl varchar(500) NULL,
        RetrievedAt datetime NOT NULL CONSTRAINT DF_MaltProduct_RetrievedAt DEFAULT (GETDATE()),
        IsActive bit NOT NULL CONSTRAINT DF_MaltProduct_IsActive DEFAULT (1),
        CONSTRAINT FK_MaltProduct_MaltType FOREIGN KEY (MaltTypeId) REFERENCES dbo.MaltType(MaltTypeId),
        CONSTRAINT FK_MaltProduct_Fermentable FOREIGN KEY (FermentableId) REFERENCES dbo.Fermentable(FermentableId)
    );
    CREATE UNIQUE INDEX UX_MaltProduct_Manufacturer_Product
        ON dbo.MaltProduct (ManufacturerName, ProductName);
END;

IF OBJECT_ID(N'dbo.WaterAddition', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WaterAddition (
        WaterAdditionId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name varchar(150) NOT NULL,
        Formula varchar(100) NULL,
        Description varchar(1000) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_WaterAddition_IsActive DEFAULT (1),
        CONSTRAINT UX_WaterAddition_Name UNIQUE (Name)
    );
END;
