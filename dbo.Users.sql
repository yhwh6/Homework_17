CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [lastName] NVARCHAR(50) NOT NULL, 
    [firstName] NVARCHAR(50) NOT NULL, 
    [middleName] NVARCHAR(50) NOT NULL, 
    [phoneNumber] NVARCHAR(50) NULL, 
    [email] NVARCHAR(50) NOT NULL
)

SET IDENTITY_INSERT [dbo].[Users] ON

DECLARE @Counter INT = 1

WHILE @Counter <= 300
BEGIN
    INSERT INTO [dbo].[Users] ([lastName], [firstName], [middleName], [phoneNumber], [email]) 
    VALUES (N'LastName' + CAST(@Counter AS NVARCHAR(50)), 
            N'FirstName' + CAST(@Counter AS NVARCHAR(50)), 
            N'MiddleName' + CAST(@Counter AS NVARCHAR(50)), 
            N'PhoneNumber' + CAST(@Counter AS NVARCHAR(50)), 
            N'Email' + CAST(@Counter AS NVARCHAR(50)))
    
    SET @Counter = @Counter + 1
END

SET IDENTITY_INSERT [dbo].[Users] OFF
