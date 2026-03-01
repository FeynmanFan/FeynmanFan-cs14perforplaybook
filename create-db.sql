-- Create database (optional - skip if you already have one)
CREATE DATABASE ProductPlayground;
GO

USE ProductPlayground;
GO

-- Create the table
CREATE TABLE Products
(
    ID              INT             IDENTITY(1,1)   PRIMARY KEY,
    Name            NVARCHAR(150)   NOT NULL,
    Description     NVARCHAR(500)   NULL,
    Price           DECIMAL(18,2)   NOT NULL,
    IsDiscounted    BIT             NOT NULL        CONSTRAINT DF_Products_IsDiscounted DEFAULT 0,
    StockQuantity   INT             NOT NULL        CONSTRAINT DF_Products_Stock DEFAULT 0,
    CreatedAt       DATETIME2       NOT NULL        CONSTRAINT DF_Products_CreatedAt DEFAULT SYSUTCDATETIME(),
    LastUpdatedAt   DATETIME2       NULL
);
GO

-- Optional useful indexes
CREATE INDEX IX_Products_IsDiscounted   ON Products(IsDiscounted);
CREATE INDEX IX_Products_Price          ON Products(Price);
GO