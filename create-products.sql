USE ProductPlayground;
GO

-- TRUNCATE TABLE Products;

WITH Numbers AS
(
    SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM master..spt_values a
    CROSS JOIN master..spt_values b
    WHERE a.number < 100
)
INSERT INTO Products
    (Name, Description, Price, IsDiscounted, StockQuantity)
SELECT 
    'Product ' + RIGHT('000' + CAST(n AS VARCHAR(10)), 3) AS Name,
    'Description for product number ' + CAST(n AS VARCHAR(10)) AS Description,
    ROUND(10 + (RAND(CHECKSUM(NEWID())) * 490), 2) AS Price, 
    CASE WHEN n % 7 = 0 THEN 1 ELSE 0 END AS IsDiscounted, 
    ABS(CHECKSUM(NEWID())) % 500 + 10 AS StockQuantity 
FROM Numbers;
GO


SELECT TOP 10 * FROM Products ORDER BY ID;
SELECT COUNT(*) AS TotalProducts, 
       SUM(CASE WHEN IsDiscounted = 1 THEN 1 ELSE 0 END) AS DiscountedCount
FROM Products;
GO