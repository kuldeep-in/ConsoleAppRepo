
IF OBJECT_ID('FKConstraint') IS NULL
--DROP TABLE FKConstraint
--GO
BEGIN
CREATE TABLE FKConstraint
    (
        [SchName]	NVARCHAR(20),
		TableName	NVARCHAR(200),
		[FKName]		NVARCHAR(MAX),
		DropScript   NVARCHAR(MAX) ,
        CreateScript NVARCHAR(MAX)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM FKConstraint)
BEGIN


INSERT INTO FKConstraint
    (
		[SchName],
		TableName,
		[FKName],
        [DropScript] ,
        [CreateScript]
    )
SELECT   
		--is_not_trusted,
		schm.[name] as [SchName]
		, [paerntObj].[name] AS TableName
		,fk.[name] AS [FKName],
		 'ALTER TABLE [' + schm.[name] + '].[' + [paerntObj].[name] + '] DROP CONSTRAINT ' + '[' + fk.[name] + '] ' ,
         'ALTER TABLE [' + schm.[name] + '].[' + [paerntObj].[name] + ']'+ CASE WHEN fk.is_not_trusted = 0 
																				THEN '' 
																				ELSE ' WITH NOCHECK' 
																				--ELSE ' ' 
																			END 
																		+' ADD CONSTRAINT ' + '[' + fk.[name] + '] FOREIGN KEY ( ' +
             (
                 SELECT   COL_NAME(srcFK.parent_object_id, srcFK.parent_column_id) + CASE
                                                                                         WHEN srcFK.constraint_column_id <
                                                                                             (
                                                                                                 SELECT MAX(constraint_column_id)
                                                                                                 FROM   sys.foreign_key_columns srcFKInner
                                                                                                 WHERE  srcFKInner.constraint_object_id = fk.object_id
                                                                                             ) THEN
                                                                                             ', '
                                                                                         ELSE
                                                                                             ''
                                                                                     END
                 FROM     sys.foreign_key_columns srcFK
                 WHERE    srcFK.constraint_object_id = fk.object_id
                 ORDER BY srcFK.constraint_column_id
                 FOR XML PATH('')
             ) + ' ) ' + 'REFERENCES [' + [refSchema].[name] + '].[' + [refObj].[name] + '] ( ' +
             (
                 SELECT   COL_NAME(refFK.referenced_object_id, refFK.referenced_column_id) + CASE
                                                                                                 WHEN refFK.constraint_column_id <
                                                                                                     (
                                                                                                         SELECT MAX(constraint_column_id)
                                                                                                         FROM   sys.foreign_key_columns refFKInner
                                                                                                         WHERE  refFKInner.constraint_object_id = fk.object_id
                                                                                                     ) THEN
                                                                                                     ', '
                                                                                                 ELSE
                                                                                                     ''
                                                                                             END
                 FROM     sys.foreign_key_columns refFK
                 WHERE    refFK.constraint_object_id = fk.object_id
                 ORDER BY refFK.constraint_column_id
                 FOR XML PATH('')
             ) + ' ); '
			 --,*
FROM     sys.foreign_keys fk
         INNER JOIN sys.[objects] paerntObj
                 ON [paerntObj].[object_id] = [fk].[parent_object_id] --and object_name([paerntObj].object_id) = 'Engagement'
         INNER JOIN sys.[schemas] schm
                 ON [schm].[schema_id] = [paerntObj].[schema_id]
         INNER JOIN sys.[objects] refObj
                 ON refObj.[object_id] = [fk].[referenced_object_id]
         INNER JOIN sys.[schemas] refSchema
                 ON refSchema.[schema_id] = [refObj].[schema_id]

ORDER BY fk.name;

END
GO

--select * from FKConstraint

IF OBJECT_ID('tempdb..#temp') IS NOT NULL
DROP TABLE #temp
GO

IF OBJECT_ID('tempdb..#temp_errors_drop') IS NOT NULL
DROP TABLE #temp_errors_drop
GO

IF OBJECT_ID('tempdb..#temp_errors_create') IS NOT NULL
DROP TABLE #temp_errors_create
GO

CREATE TABLE #temp (id INT IDENTITY(1,1), DropScript NVARCHAR(MAX),CreateScript NVARCHAR(MAX), FKName VARCHAR(300), SchName NVARCHAR(20))
GO

CREATE TABLE #temp_errors_drop(erMsg NVARCHAR(MAX))
GO

CREATE TABLE #temp_errors_create(erMsg NVARCHAR(MAX))
GO

INSERT INTO #temp(DropScript,CreateScript, FKName, SchName )
SELECT DropScript, CreateScript, FKName, SchName from FKConstraint

GO
--select * from #temp

DECLARE @iterator INT =1
DECLARE @id INT
DECLARE @FKName VARCHAR(300)
DECLARE @scrpt NVARCHAR(MAX)
DECLARE @SchName NVARCHAR(20)

-- drop FK

WHILE EXISTS (SELECT 1 FROM #temp)--(1=1)
	BEGIN
		
		SELECT @id = id, @scrpt = [DropScript], @FKName = FKName, @SchName = SchName FROM #temp WHERE id = @iterator
		IF (@iterator != @id )
		BREAK;


		BEGIN TRY
		
			--print @scrpt
			IF EXISTS (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = @FKName)
			BEGIN
				PRINT 'Dropping FK - '+ QUOTENAME(@SchName)+'.'+QUOTENAME(@FKName) + '...'
				EXEC (@scrpt)
			END

		END TRY

		BEGIN CATCH
		
			PRINT ERROR_MESSAGE()
			INSERT INTO #temp_errors_drop
				SELECT ERROR_MESSAGE()
			
		END CATCH

		SET @iterator = @iterator + 1
		
	END

	IF NOT EXISTS (SELECT 1 FROm #temp)
		BEGIN
			PRINT 'There no FKs to Drop...'
		END
	ELSE IF NOT EXISTS(SELECT 1 FROM #temp_errors_drop)
		BEGIN
			PRINT 'All FKs Dropped successfully...'
		END
	ELSE
		BEGIN
			PRINT 'There are some problems while Dropping FKs... Please see the ouput. '
			--SELECT * INTO FK_Errors FROM #temp_errors
			select * from #temp_errors_drop

		END
GO
	-- put delete logic here

	EXEC [dbo].[usp_DeleteEngagement] @EngagementId ='7B174C3A-8BD7-E811-A8EB-0003FF74C578 ', @cleanOperations = 1 

	-- end of delete logic
GO
--Create FK

DECLARE @iterator INT =1
DECLARE @id INT
DECLARE @FKName VARCHAR(300)
DECLARE @scrpt NVARCHAR(MAX)
DECLARE @SchName NVARCHAR(20)

SET @iterator = 1

	WHILE EXISTS (SELECT 1 FROM #temp)--(1=1)
	BEGIN
		
		SELECT @id = id, @scrpt = [CreateScript], @FKName = FKName, @SchName = SchName FROM #temp WHERE id = @iterator
		IF (@iterator != @id )
		BREAK;

		BEGIN TRY
		
			--print @scrpt
			IF NOT EXISTS (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = @FKName AND CONSTRAINT_SCHEMA = @SchName)
			BEGIN
				PRINT 'Creating FK - '+QUOTENAME(@SchName) + '.' + QUOTENAME(@FKName) + '...'
				EXEC (@scrpt)
			END

		END TRY

		BEGIN CATCH
		
			PRINT ERROR_MESSAGE()
			INSERT INTO #temp_errors_create
				SELECT ERROR_MESSAGE()
			
		END CATCH

		SET @iterator = @iterator + 1
		
	END

	IF NOT EXISTS (SELECT 1 FROm #temp)
		BEGIN
			PRINT 'There no FKs to Create...'
		END
	ELSE IF NOT EXISTS(SELECT 1 FROM #temp_errors_create)
		BEGIN
			PRINT 'All FKs created successfully...'
		END
	ELSE
		BEGIN
			PRINT 'There are some problems while creating FKs... Please see the ouput. '
			--SELECT * INTO FK_Errors FROM #temp_errors
			select * from #temp_errors_create

		END
GO