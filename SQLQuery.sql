create table Customer
(
CustomerID INT PRIMARY KEY IDENTITY,
Name VARCHAR(100),
Email VARCHAR(100),
Phone VARCHAR(20)
)
go

CREATE TABLE Product 
(
ProductID INT PRIMARY KEY IDENTITY,
Name VARCHAR(100),
Price DECIMAL,
);
go

Create table Invoice
(
InvoiceID INT Primary key identity,
CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID),
InvoiceDate DATETIME DEFAULT GETDATE(),
TotalAmount DECIMAL
)
GO

CREATE TABLE InvoiceDetail
(
InvoiceDetailID INT PRIMARY KEY IDENTITY,
InvoiceID INT FOREIGN KEY REFERENCES Invoice(InvoiceID),
ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
Quantity INT,
UnitPrice DECIMAL,
SubTotal DECIMAL
)


USE InvoiceSystemDB;
GO

CREATE PROCEDURE sp_InsertProduct
	@Name VARCHAR(100),
	@Price DECIMAL
AS
BEGIN
	INSERT INTO Product (Name, Price)
	VALUES (@Name, @Price);
    
    SELECT SCOPE_IDENTITY() AS NewProductID;
END
GO
CREATE PROCEDURE sp_InsertCustomer
	@Name VARCHAR(100),
	@Email VARCHAR(100),
	@Phone VARCHAR(20)
AS
BEGIN 
	INSERT INTO Customer (Name, Email, Phone) 
	VALUES (@Name, @Email, @Phone);

    SELECT SCOPE_IDENTITY() AS NewCustomerID;
END
GO

CREATE TYPE InvoiceDetailType AS TABLE
(
    ProductID INT,
    Quantity INT,
    UnitPrice DECIMAL(10, 2),
    SubTotal DECIMAL(10, 2)
);
GO

USE InvoiceSystemDB;
GO

-- 
CREATE PROCEDURE sp_CreateInvoice
    @CustomerID INT,
    @InvoiceDate DATETIME,
    @Details InvoiceDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;

    DECLARE @NewInvoiceID INT;
    DECLARE @CalculatedTotalAmount DECIMAL(10, 2);

    SELECT @CalculatedTotalAmount = SUM(SubTotal) 
    FROM @Details;

    BEGIN TRY
        INSERT INTO Invoice (CustomerID, InvoiceDate, TotalAmount)
        VALUES (@CustomerID, @InvoiceDate, @CalculatedTotalAmount);

        SET @NewInvoiceID = SCOPE_IDENTITY();

        INSERT INTO InvoiceDetail (InvoiceID, ProductID, Quantity, UnitPrice, SubTotal)
        SELECT @NewInvoiceID, ProductID, Quantity, UnitPrice, SubTotal
        FROM @Details;

        COMMIT TRANSACTION;

        SELECT @NewInvoiceID AS NewInvoiceID;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW; 
        
        SELECT 0 AS NewInvoiceID;

    END CATCH
END
GO

USE InvoiceSystemDB;
GO

CREATE PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SELECT CustomerID, Name FROM Customer ORDER BY Name;
END
GO

CREATE PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT ProductID, Name, Price FROM Product ORDER BY Name;
END
GO


-------------------------Invoice List
USE InvoiceSystemDB;
GO

CREATE PROCEDURE dbo.sp_GetInvoicesByCustomer
    @SearchTerm VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @SearchTerm IS NULL OR LTRIM(RTRIM(@SearchTerm)) = ''
    BEGIN
        SET @SearchTerm = '%';
    END
    ELSE
    BEGIN
        SET @SearchTerm = '%' + @SearchTerm + '%';
    END

    SELECT 
        I.InvoiceID, 
        I.InvoiceDate, 
        I.TotalAmount,
        C.CustomerID,
        C.Name AS CustomerName
    FROM dbo.Invoice I
    INNER JOIN dbo.Customer C ON I.CustomerID = C.CustomerID
    WHERE C.Name LIKE @SearchTerm OR CAST(I.InvoiceID AS VARCHAR) LIKE @SearchTerm
    ORDER BY I.InvoiceDate DESC;
END
GO


----------Invoice Information
USE InvoiceSystemDB;
GO

CREATE PROCEDURE sp_GetInvoiceDataForReport
    @InvoiceID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        I.InvoiceID,
        I.InvoiceDate,
        I.TotalAmount,
        C.Name AS CustomerName,
        C.Email AS CustomerEmail,
        C.Phone AS CustomerPhone
    FROM dbo.Invoice I
    INNER JOIN dbo.Customer C ON I.CustomerID = C.CustomerID
    WHERE I.InvoiceID = @InvoiceID;

    SELECT
        ID.Quantity,
        ID.UnitPrice,
        ID.SubTotal,
        P.Name AS ProductName
    FROM dbo.InvoiceDetail ID
    INNER JOIN dbo.Product P ON ID.ProductID = P.ProductID
    WHERE ID.InvoiceID = @InvoiceID;
END
GO

