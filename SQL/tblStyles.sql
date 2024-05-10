CREATE TABLE [aidemo].[styles] (
                                   id              int NOT NULL PRIMARY KEY CLUSTERED,
                                   gender          nvarchar(50) NOT NULL,
    masterCategory  nvarchar(50) NOT NULL,
    subCategory     nvarchar(50) NOT NULL,
    articleType     nvarchar(50) NOT NULL,
    baseColour      nvarchar(50) NOT NULL,
    season          nvarchar(50) NOT NULL,
    year            smallint,
    usage           nvarchar(50) NOT NULL,
    productDisplayName  nvarchar(100) NOT NULL
    )