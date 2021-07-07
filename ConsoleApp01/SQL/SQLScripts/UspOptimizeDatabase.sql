
----
----
CREATE PROCEDURE [UTIL].[UspOptimizeDatabase]
AS
    BEGIN
        BEGIN TRY
            SET NOCOUNT ON;

            IF EXISTS (
                          SELECT   1
                             FROM  [internal].[DatabaseConfiguration]
                             WHERE [Name] = 'IsOptimizationRunning'
                             AND   [Value] = '1'
							 AND DATEDIFF(mi,ModifiedOn,GETUTCDATE()) < 150
                      )
                BEGIN
                    PRINT 'Optimization is already running...';
                    RETURN;
                END;

            PRINT 'Optimizating database ...';
            UPDATE   [internal].[DatabaseConfiguration]
               SET   [Value] = '1' ,
                     [ModifiedOn] = GETUTCDATE()
               WHERE [Name] = 'IsOptimizationRunning';

            DECLARE @indexName NVARCHAR(256) ,
                    @tableName NVARCHAR(256) ,
                    @schemaName NVARCHAR(256) ,
                    @IndexReuildScript NVARCHAR(MAX);

            PRINT 'Index Rebuild Started';
            DECLARE cursor_IndexRebuild CURSOR FOR
            SELECT   DISTINCT
                     sc.Name ,
                     OBJECT_NAME(indexes.object_id) AS TableName ,
                     indexes.name AS IndexName
               FROM  sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') ps
                     INNER JOIN sys.objects objects
                             ON objects.object_id = ps.object_id
                     INNER JOIN sys.indexes indexes
                             ON objects.object_id = indexes.object_id
                     INNER JOIN sys.schemas sc
                             ON sc.Schema_id = objects.Schema_id
               WHERE (
                         (
                             avg_fragmentation_in_percent > 30
                      AND    ps.index_id > 0
                         )
                  AND    (avg_page_space_used_in_percent < 85)
                     )
               AND   objects.type = 'U';

            DECLARE @AlterIndexOptions NVARCHAR(MAX);

            IF SERVERPROPERTY('Edition') = 'SQL Azure'
                SET @AlterIndexOptions = N'REBUILD WITH (FILLFACTOR = 80, ONLINE=ON)';
            ELSE
                SET @AlterIndexOptions = N'REBUILD WITH (FILLFACTOR = 80)';

            OPEN cursor_IndexRebuild;
            FETCH NEXT FROM cursor_IndexRebuild
            INTO @schemaName ,
                 @tableName ,
                 @indexName;

            WHILE @@FETCH_STATUS = 0
                BEGIN
                    SELECT @IndexReuildScript = 'ALTER INDEX [' + @indexName + '] ON [' + @schemaName + '].[' + @tableName + '] ' + @AlterIndexOptions;
                    EXECUTE sp_executesql @IndexReuildScript;
                    IF SERVERPROPERTY('Edition') = 'SQL Azure'
                        WAITFOR DELAY '00:00:03';
                    FETCH NEXT FROM cursor_IndexRebuild
                    INTO @schemaName ,
                         @tableName ,
                         @indexName;
                    SET @IndexReuildScript = '';
                END;

            CLOSE cursor_IndexRebuild;
            DEALLOCATE cursor_IndexRebuild;
            PRINT 'Index Rebuild Completed';
            -- Index Rebuild Completed ----------------------------------------------

            -- Statistics Update Started --------------------------------------------
            PRINT 'Statistics Update Started';
            DECLARE @SchName NVARCHAR(256) ,
                    @tblName NVARCHAR(256) ,
                    @IndexStatsName NVARCHAR(256) ,
                    @updateStatScript NVARCHAR(MAX);

            DECLARE cursor_UpdateStats CURSOR FOR
            SELECT  DISTINCT
                    sc.name ,
                    OBJECT_NAME(ss.object_id) ,
                    ss.name
               FROM sys.stats ss
                    INNER JOIN sys.objects so
                            ON  so.object_id = ss.object_id
                            AND so.type = 'U'
                            AND so.schema_id = 1
                    INNER JOIN sys.indexes si
                            ON si.object_id = so.object_id
                    INNER JOIN sys.schemas sc
                            ON sc.schema_id = so.schema_id;

            OPEN cursor_UpdateStats;

            FETCH NEXT FROM cursor_UpdateStats
            INTO @SchName ,
                 @tblName ,
                 @IndexStatsName;

            WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @updateStatScript = '';
                    SELECT @updateStatScript = 'UPDATE STATISTICS [' + @SchName + '].[' + @tblName + ']' + @IndexStatsName + ' WITH FULLSCAN';
                    EXECUTE sp_executesql @updateStatScript;
                    FETCH NEXT FROM cursor_UpdateStats
                    INTO @SchName ,
                         @tblName ,
                         @IndexStatsName;
                END;

            CLOSE cursor_UpdateStats;
            DEALLOCATE cursor_UpdateStats;
            PRINT 'Statistics Update Completed';
            -- Statistics Update Completed --------------------------------

            UPDATE   [internal].[DatabaseConfiguration]
               SET   [Value] = '0' ,
                     [ModifiedOn] = GETUTCDATE()
               WHERE [Name] = 'IsOptimizationRunning';

        END TRY
        BEGIN CATCH
            UPDATE   [internal].[DatabaseConfiguration]
               SET   [Value] = '0' ,
                     [ModifiedOn] = GETUTCDATE()
               WHERE [Name] = 'IsOptimizationRunning';
            DECLARE @errMsg NVARCHAR(4000) ,
                    @errSeverity INT;
            SELECT @errMsg = ERROR_MESSAGE() ,
                   @errSeverity = ERROR_SEVERITY();
            RAISERROR(@errMsg, @errSeverity, 1);
        END CATCH;
    END;
