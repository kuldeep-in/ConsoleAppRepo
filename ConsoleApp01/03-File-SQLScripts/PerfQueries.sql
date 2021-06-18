-----------------------------------------------------
-- Find Wait statistics -----------------------------
-----------------------------------------------------
WITH [Waits] 
AS (SELECT wait_type, wait_time_ms/ 1000.0 AS [WaitS],
          (wait_time_ms - signal_wait_time_ms) / 1000.0 AS [ResourceS],
           signal_wait_time_ms / 1000.0 AS [SignalS],
           waiting_tasks_count AS [WaitCount],
           100.0 *  wait_time_ms / SUM (wait_time_ms) OVER() AS [Percentage],
           ROW_NUMBER() OVER(ORDER BY wait_time_ms DESC) AS [RowNum]
    FROM sys.dm_db_wait_stats WITH (NOLOCK)
    WHERE [wait_type] NOT IN (
        N'BROKER_EVENTHANDLER', N'BROKER_RECEIVE_WAITFOR', N'BROKER_TASK_STOP',
		N'BROKER_TO_FLUSH', N'BROKER_TRANSMITTER', N'CHECKPOINT_QUEUE',
        N'CHKPT', N'CLR_AUTO_EVENT', N'CLR_MANUAL_EVENT', N'CLR_SEMAPHORE',
        N'DBMIRROR_DBM_EVENT', N'DBMIRROR_EVENTS_QUEUE', N'DBMIRROR_WORKER_QUEUE',
		N'DBMIRRORING_CMD', N'DIRTY_PAGE_POLL', N'DISPATCHER_QUEUE_SEMAPHORE',
        N'EXECSYNC', N'FSAGENT', N'FT_IFTS_SCHEDULER_IDLE_WAIT', N'FT_IFTSHC_MUTEX',
        N'HADR_CLUSAPI_CALL', N'HADR_FILESTREAM_IOMGR_IOCOMPLETION', N'HADR_LOGCAPTURE_WAIT', 
		N'HADR_NOTIFICATION_DEQUEUE', N'HADR_TIMER_TASK', N'HADR_WORK_QUEUE',
        N'KSOURCE_WAKEUP', N'LAZYWRITER_SLEEP', N'LOGMGR_QUEUE', 
		N'MEMORY_ALLOCATION_EXT', N'ONDEMAND_TASK_QUEUE',
		N'PREEMPTIVE_HADR_LEASE_MECHANISM', N'PREEMPTIVE_SP_SERVER_DIAGNOSTICS',
		N'PREEMPTIVE_ODBCOPS',
		N'PREEMPTIVE_OS_LIBRARYOPS', N'PREEMPTIVE_OS_COMOPS', N'PREEMPTIVE_OS_CRYPTOPS',
		N'PREEMPTIVE_OS_PIPEOPS', N'PREEMPTIVE_OS_AUTHENTICATIONOPS',
		N'PREEMPTIVE_OS_GENERICOPS', N'PREEMPTIVE_OS_VERIFYTRUST',
		N'PREEMPTIVE_OS_FILEOPS', N'PREEMPTIVE_OS_DEVICEOPS', N'PREEMPTIVE_OS_QUERYREGISTRY',
		N'PREEMPTIVE_OS_WRITEFILE',
		N'PREEMPTIVE_XE_CALLBACKEXECUTE', N'PREEMPTIVE_XE_DISPATCHER',
		N'PREEMPTIVE_XE_GETTARGETSTATE', N'PREEMPTIVE_XE_SESSIONCOMMIT',
		N'PREEMPTIVE_XE_TARGETINIT', N'PREEMPTIVE_XE_TARGETFINALIZE',
		N'PREEMPTIVE_XHTTP',
        N'PWAIT_ALL_COMPONENTS_INITIALIZED', N'PWAIT_DIRECTLOGCONSUMER_GETNEXT',
		N'QDS_PERSIST_TASK_MAIN_LOOP_SLEEP',
		N'QDS_ASYNC_QUEUE',
        N'QDS_CLEANUP_STALE_QUERIES_TASK_MAIN_LOOP_SLEEP', N'REQUEST_FOR_DEADLOCK_SEARCH',
		N'RESOURCE_GOVERNOR_IDLE',
		N'RESOURCE_QUEUE', N'SERVER_IDLE_CHECK', N'SLEEP_BPOOL_FLUSH', N'SLEEP_DBSTARTUP',
		N'SLEEP_DCOMSTARTUP', N'SLEEP_MASTERDBREADY', N'SLEEP_MASTERMDREADY',
        N'SLEEP_MASTERUPGRADED', N'SLEEP_MSDBSTARTUP', N'SLEEP_SYSTEMTASK', N'SLEEP_TASK',
        N'SLEEP_TEMPDBSTARTUP', N'SNI_HTTP_ACCEPT', N'SP_SERVER_DIAGNOSTICS_SLEEP',
		N'SQLTRACE_BUFFER_FLUSH', N'SQLTRACE_INCREMENTAL_FLUSH_SLEEP', N'SQLTRACE_WAIT_ENTRIES',
		N'WAIT_FOR_RESULTS', N'WAITFOR', N'WAITFOR_TASKSHUTDOWN', N'WAIT_XTP_HOST_WAIT',
		N'WAIT_XTP_OFFLINE_CKPT_NEW_LOG', N'WAIT_XTP_CKPT_CLOSE', N'WAIT_XTP_RECOVERY',
		N'XE_BUFFERMGR_ALLPROCESSED_EVENT', N'XE_DISPATCHER_JOIN',
        N'XE_DISPATCHER_WAIT', N'XE_LIVE_TARGET_TVF', N'XE_TIMER_EVENT')
    AND waiting_tasks_count > 0)
SELECT
    MAX (W1.wait_type) AS [WaitType],
	CAST (MAX (W1.Percentage) AS DECIMAL (5,2)) AS [Wait Percentage],
	CAST ((MAX (W1.WaitS) / MAX (W1.WaitCount)) AS DECIMAL (16,4)) AS [AvgWait_Sec],
    CAST ((MAX (W1.ResourceS) / MAX (W1.WaitCount)) AS DECIMAL (16,4)) AS [AvgRes_Sec],
    CAST ((MAX (W1.SignalS) / MAX (W1.WaitCount)) AS DECIMAL (16,4)) AS [AvgSig_Sec],
    CAST (MAX (W1.WaitS) AS DECIMAL (16,2)) AS [Total_Wait_Sec],
    CAST (MAX (W1.ResourceS) AS DECIMAL (16,2)) AS [Resource_Sec],
    CAST (MAX (W1.SignalS) AS DECIMAL (16,2)) AS [Signal_Sec],
    MAX (W1.WaitCount) AS [Wait Count]   
FROM Waits AS W1
INNER JOIN Waits AS W2
ON W2.RowNum <= W1.RowNum
GROUP BY W1.RowNum
HAVING SUM (W2.Percentage) - MAX (W1.Percentage) < 99 -- percentage threshold
OPTION (RECOMPILE);


-----------------------------------------------------------
-- Query execution Stats ----------------------------------
-----------------------------------------------------------
SELECT TOP(50) DB_NAME(t.[dbid]) AS [Database Name], 
REPLACE(REPLACE(LEFT(t.[text], 1000), CHAR(10),''), CHAR(13),'') AS [Short Query Text],  
qs.total_worker_time AS [Total Worker Time], qs.min_worker_time AS [Min Worker Time],
qs.total_worker_time/qs.execution_count AS [Avg Worker Time], 
qs.max_worker_time AS [Max Worker Time], 
qs.min_elapsed_time AS [Min Elapsed Time], 
qs.total_elapsed_time/qs.execution_count AS [Avg Elapsed Time], 
qs.max_elapsed_time AS [Max Elapsed Time],
qs.min_logical_reads AS [Min Logical Reads],
qs.total_logical_reads/qs.execution_count AS [Avg Logical Reads],
qs.max_logical_reads AS [Max Logical Reads], 
qs.execution_count AS [Execution Count], qs.creation_time AS [Creation Time]
--,t.[text] AS [Query Text], qp.query_plan AS [Query Plan] -- uncomment out these columns if not copying results to Excel
FROM sys.dm_exec_query_stats AS qs WITH (NOLOCK)
CROSS APPLY sys.dm_exec_sql_text(plan_handle) AS t 
CROSS APPLY sys.dm_exec_query_plan(plan_handle) AS qp
WHERE t.dbid = DB_ID() 
ORDER BY qs.total_worker_time DESC OPTION (RECOMPILE);


-----------------------------------------------------
-- Long Running Stored Procedures -------------------
-----------------------------------------------------
SELECT DISTINCT TOP 20
            OBJECT_NAME(QSQ.object_id) AS [ProcedureName],
            QSQT.query_sql_text ,
            CAST(QSRS.max_duration AS FLOAT) / 1000000 AS MaxDuration ,
            QSRS.avg_duration / 1000000 AS AvgDuration ,
            QSRS.last_execution_time ,
            QSRS.count_executions
   FROM     sys.query_store_query_text QSQT
            INNER JOIN sys.query_store_query QSQ
                    ON QSQT.query_text_id = QSQ.query_text_id
            INNER JOIN sys.query_store_plan QSP
                    ON QSP.query_id = QSQ.query_id
            INNER JOIN sys.query_store_runtime_stats QSRS
                    ON QSRS.plan_id = QSP.plan_id
   WHERE    QSRS.max_duration / 1000000 < 30
   AND      QSRS.last_execution_time > DATEADD(HOUR, -50, GETDATE())
   --AND      QSRS.last_execution_time < GETDATE()
   AND OBJECT_NAME(QSQ.object_id) IS NOT NULL
   ORDER BY MaxDuration DESC;
GO

---------------------------------------------------
-- Space used by Database Objects -----------------
---------------------------------------------------
sp_spaceused [ResponseApplicationVariableInstance]
-- Calculates the size of individual database objects.
SELECT sys.objects.name, SUM(reserved_page_count) * 8.0 / 1024
FROM sys.dm_db_partition_stats, sys.objects
WHERE sys.dm_db_partition_stats.object_id = sys.objects.object_id
GROUP BY sys.objects.name;
GO


---------------------------------------------------
-- Space used by Indexes --------------------------
---------------------------------------------------
SELECT
OBJECT_NAME(i.OBJECT_ID) AS TableName,
i.name AS IndexName,
i.index_id AS IndexID,
8 * SUM(a.used_pages) AS 'Indexsize(KB)'
FROM sys.indexes AS i
JOIN sys.partitions AS p ON p.OBJECT_ID = i.OBJECT_ID AND p.index_id = i.index_id
JOIN sys.allocation_units AS a ON a.container_id = p.partition_id
GROUP BY i.OBJECT_ID,i.index_id,i.name
ORDER BY [Indexsize(KB)] desc
--ORDER BY OBJECT_NAME(i.OBJECT_ID),i.index_id


--------------------------------------------------
-- Write Cursor ----------------------------------
--------------------------------------------------
-- Loop for all publishedversionids
DECLARE @memberFirmId AS TABLE ( Id BIGINT );
DECLARE @engagementId UNIQUEIDENTIFIER;

DECLARE cur CURSOR FOR
SELECT [EngagementId]
FROM   [dbo].[EngagementInformation]
       INNER JOIN @memberFirmId
               ON [@memberFirmId].[Id] = [dbo].[EngagementInformation].[MemberFirmId];

OPEN cur;

FETCH NEXT FROM cur
INTO @engagementId;

WHILE @@FETCH_STATUS = 0
    BEGIN

	PRINT @engagementId;

        FETCH NEXT FROM cur
        INTO @engagementId;
    END;

CLOSE cur;
DEALLOCATE cur;


---------------------------------------------------------
-- Query Store properties -------------------------------
---------------------------------------------------------
SELECT * FROM sys.database_query_store_options;

--------------------------------------------------------
-- Queries and plans -----------------------------------
--------------------------------------------------------
SELECT Txt.query_text_id, Txt.query_sql_text, Pl.plan_id, Qry.*
FROM sys.query_store_plan AS Pl
INNER JOIN sys.query_store_query AS Qry
    ON Pl.query_id = Qry.query_id
INNER JOIN sys.query_store_query_text AS Txt
    ON Qry.query_text_id = Txt.query_text_id ;


---------------------------------------------------------
-- Get Query Execution plan in detail -------------------
---------------------------------------------------------
SELECT cp.objtype AS ObjectType,
OBJECT_NAME(st.objectid,st.dbid) AS ObjectName,
cp.usecounts AS ExecutionCount,
st.TEXT AS QueryText,
qp.query_plan AS QueryPlan
FROM sys.dm_exec_cached_plans AS cp
CROSS APPLY sys.dm_exec_query_plan(cp.plan_handle) AS qp
CROSS APPLY sys.dm_exec_sql_text(cp.plan_handle) AS st
where OBJECT_NAME(st.objectid,st.dbid) like 'spload%'


---------------------------------------------------------
-- Top 20 stored procedures -----------------------------
---------------------------------------------------------
SELECT DISTINCT TOP 20
            OBJECT_NAME(QSQ.object_id) AS [ProcedureName],
            QSQT.query_sql_text ,
            CAST(QSRS.max_duration AS FLOAT) / 1000000 AS MaxDuration ,
            QSRS.avg_duration / 1000000 AS AvgDuration ,
            QSRS.last_execution_time ,
            QSRS.count_executions
   FROM     sys.query_store_query_text QSQT
            INNER JOIN sys.query_store_query QSQ
                    ON QSQT.query_text_id = QSQ.query_text_id
            INNER JOIN sys.query_store_plan QSP
                    ON QSP.query_id = QSQ.query_id
            INNER JOIN sys.query_store_runtime_stats QSRS
                    ON QSRS.plan_id = QSP.plan_id
   WHERE    QSRS.max_duration / 1000000 < 30
   --AND      QSRS.last_execution_time > DATEADD(HOUR, -50, GETDATE())
   --AND      QSRS.last_execution_time < GETDATE()
   AND OBJECT_NAME(QSQ.object_id) IS NOT NULL
   ORDER BY MaxDuration DESC;


---------------------------------------------------------
-- Longest avg execution time ---------------------------
---------------------------------------------------------
SELECT TOP 10 rs.avg_duration, qt.query_sql_text, q.query_id,
    qt.query_text_id, p.plan_id, GETUTCDATE() AS CurrentUTCTime,
    rs.last_execution_time
FROM sys.query_store_query_text AS qt
JOIN sys.query_store_query AS q
    ON qt.query_text_id = q.query_text_id
JOIN sys.query_store_plan AS p
    ON q.query_id = p.query_id
JOIN sys.query_store_runtime_stats AS rs
    ON p.plan_id = rs.plan_id
WHERE rs.last_execution_time > DATEADD(hour, -1, GETUTCDATE())
ORDER BY rs.avg_duration DESC;


---------------------------------------------------------
-- Max I/O reads with execution count--------------------
---------------------------------------------------------
SELECT TOP 10 rs.avg_physical_io_reads, qt.query_sql_text,
    q.query_id, qt.query_text_id, p.plan_id, rs.runtime_stats_id,
    rsi.start_time, rsi.end_time, rs.avg_rowcount, rs.count_executions
FROM sys.query_store_query_text AS qt
JOIN sys.query_store_query AS q
    ON qt.query_text_id = q.query_text_id
JOIN sys.query_store_plan AS p
    ON q.query_id = p.query_id
JOIN sys.query_store_runtime_stats AS rs
    ON p.plan_id = rs.plan_id
JOIN sys.query_store_runtime_stats_interval AS rsi
    ON rsi.runtime_stats_interval_id = rs.runtime_stats_interval_id
WHERE rsi.start_time >= DATEADD(hour, -24, GETUTCDATE())
ORDER BY rs.avg_physical_io_reads DESC;

---------------------------------------------------------
-- Index usage: SCAN/SEEK/LOOKUP ------------------------
---------------------------------------------------------
SELECT OBJECT_NAME(IX.OBJECT_ID) Table_Name
	   ,IX.name AS Index_Name
	   ,IX.type_desc Index_Type
	   ,IXUS.user_seeks AS NumOfSeeks
	   ,IXUS.user_scans AS NumOfScans
	   ,IXUS.user_lookups AS NumOfLookups
	   ,IXUS.user_updates AS NumOfUpdates
	   ,IXUS.last_user_seek AS LastSeek
	   ,IXUS.last_user_scan AS LastScan
	   ,IXUS.last_user_lookup AS LastLookup
	   ,IXUS.last_user_update AS LastUpdate
FROM sys.indexes IX
INNER JOIN sys.dm_db_index_usage_stats IXUS ON IXUS.index_id = IX.index_id AND IXUS.OBJECT_ID = IX.OBJECT_ID
WHERE OBJECTPROPERTY(IX.OBJECT_ID,'IsUserTable') = 1