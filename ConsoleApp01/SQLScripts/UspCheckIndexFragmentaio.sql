CREATE PROCEDURE [UTIL].[UspCheckIndexFragmentaion]
    @percentageFragmented INT = 30 ,
    @pageDensity INT = 85 ,
    @pageCount INT = 20
AS
    DECLARE @IsFragmented BIT;


    SELECT  OBJECT_NAME(indexstats.object_id) AS [Table Name] ,
            ind.name AS [Index Name] ,
            ROUND(indexstats.avg_fragmentation_in_percent, 2) AS [% Fragmentation] ,
            indexstats.page_count AS [Pg Count] ,
            ROUND(indexstats.avg_page_space_used_in_percent, 2) AS [Pg Density]
    FROM    sys.dm_db_index_physical_stats(DB_ID(DB_NAME()), NULL, NULL, NULL,
                                           N'DETAILED') indexstats
            CROSS APPLY sys.indexes ind
    WHERE   ind.object_id = indexstats.object_id
            AND ind.index_id = indexstats.index_id
            AND indexstats.index_level = 0
            AND indexstats.alloc_unit_type_desc = N'IN_ROW_DATA'
            AND indexstats.avg_fragmentation_in_percent > @percentageFragmented
            AND indexstats.avg_page_space_used_in_percent < @pageDensity
    ORDER BY indexstats.avg_fragmentation_in_percent DESC;

/* 
     Setting recommender variable based on fragmentation and Page Count 
*/

    IF EXISTS ( SELECT  1
                FROM    sys.dm_db_index_physical_stats(DB_ID(DB_NAME()), NULL,
                                                       NULL, NULL, N'DETAILED') indexstats
                        CROSS APPLY sys.indexes ind
                WHERE   ind.object_id = indexstats.object_id
                        AND ind.index_id = indexstats.index_id
                        AND indexstats.index_level = 0
                        AND indexstats.alloc_unit_type_desc = N'IN_ROW_DATA'
                        AND indexstats.avg_fragmentation_in_percent > @percentageFragmented
                        AND indexstats.avg_page_space_used_in_percent < @pageDensity
                        AND indexstats.page_count >= @pageCount )
        SET @IsFragmented = 1;


    ELSE
        SET @IsFragmented = 0;

    SELECT  @IsFragmented AS IsFragemented;
