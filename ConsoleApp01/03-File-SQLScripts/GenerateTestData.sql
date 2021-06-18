SET nocount ON 
CREATE TABLE orders (
OrderId BIGINT IDENTITY(1,1),
OrderDesc NVARCHAR(2000),
Quantity BIGINT,
OrderValue BIGINT
) 
--CREATE UNIQUE CLUSTERED INDEX test ON orders(d_id, o_id)

BEGIN TRAN 
DECLARE @i INT 
SET @i = 1 
WHILE @i <= 80 
BEGIN 
INSERT INTO orders 
(
OrderDesc ,
Quantity ,
OrderValue 
)
VALUES (
	REPLICATE('a', 2000),
	@i % 8, 
	RAND() * 800000
) 
SET @i = @i + 1 
END 
COMMIT TRAN

-----------------------------
-----------------------------
INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '123','Frank Martin','Oxford Street'
go 10