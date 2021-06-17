

CREATE TABLE dbo.Employee (Employee_id int,Employee_Name Varchar(200) NOT NULL,  Employee_Address Varchar(2000) NOT NULL)
GO

-- create a stored procedure
Create PROC dbo.proc_Emp_search(@Empcode int)
AS 
SET NOCOUNT ON
SELECT Employee_Name, Employee_Address FROM dbo.Employee
WHERE Employee_id= @Empcode 
GO

-- populate the table (this may take a couple of minutes)
SET NOCOUNT ON
INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '121032','Johny English','Oxford Street'
go 3000

INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '121023','Jason Bourne','Oxford Street'
go 2000  
INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '121023','Frank Martin','Oxford Street'
go 10
INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '121023','Dominic Torreto','New York Street'
go 2000
INSERT INTO [dbo].Employee (Employee_id,Employee_Name,Employee_Address) SELECT '123','Frank Martin','Oxford Street'
go 10


EXEC dbo.proc_Emp_search 123
GO 20
EXEC dbo.proc_Emp_search 121023
GO 20


-- 
/*
Test 2 - Testing with a Non Clustered Index

In this test we will create a non-clustered index and execute the stored procedure again: 

*/
CREATE NONCLUSTERED INDEX NCI_1
ON dbo.Employee (Employee_id)
GO
EXEC dbo.proc_Emp_search 123

