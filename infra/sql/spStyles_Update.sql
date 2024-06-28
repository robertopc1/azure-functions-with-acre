CREATE PROCEDURE [aidemo].[spStyles_Update]
@Id int,
@Gender nvarchar(50),
@MasterCategory nvarchar(50),
@SubCategory nvarchar(50),
@ArticleType nvarchar(50),
@BaseColour nvarchar(50),
@Season nvarchar(50),
@Year smallint,
@Usage nvarchar(50),
@ProductDisplayName nvarchar(100)
AS
BEGIN

UPDATE  [aidemo].[styles]
SET gender = @Gender,
    masterCategory = @MasterCategory,
    subCategory = @SubCategory,
    articleType = @ArticleType,
    baseColour = @BaseColour,
    season = @Season,
    [year] = @Year,
    usage = @Usage,
    productDisplayName = @ProductDisplayName
WHERE id = @Id;

END