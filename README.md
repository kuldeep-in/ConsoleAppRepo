# WP - ADF

# M05 - Mapping Data Flow
## M05L01 - Introduction to Mapping Data Flow
### Demo (At the end of Lession 02)
- 

### Knowledge Check
- How does debug mode in Data Flow differ to debug mode for an ADF pipeline?
    - Data Flow debug mode shows data in flight, allowing you to see the impact of each stage in the flow in an interactive manner. This is not available for the main ADF pipeline debug mode.
- Name two Schema Modifier activities available in Data Flow.
    - Derived Column
    - Select
    - Aggregate
    - Surrogate Key
    - Pivot
    - Unpivot
    - Window

## M05L02 - Mapping Data Flow Scenarios
### Simple Copy Flow
- You can leverage the mapping data flows to copy the data from source to sink.
- Just like the way we created pipelines with copy activity, here also you can define your source and targets independently inside a data flow.
- So this can be useful in the scenarios where you would like to do transformations during your copy process.

### Slowly Changing Dimension Scenario
- When you would like to achieve data movement or data transformation in a dynamic environment, dynamic means where your dimensions are changing, then you can use mapping data flows.
- Data flows provides you a code free environment, where you can handle the changing dimensions with various activities like, lookup, derived colums or select activities

### Load Star Schema DW Scenario
- In Star schema we stores the data in the form of fact and dimension tables.
- So if you would like to load the data to DWs where your data is defind in the form of star schema, you might have to do lot of joins and aggregations.
- Mapping data flows can be the perfect place for you to do the aggregation, where you can ingest the data from multiple data sources and then you can load that data in 
your Data warehouse.

### Data Lake Data Science
- For the data scientient, ADF provides the capabilities to visualize your data transformation, where you can monitor the transformations from either blob storage or Azure data lake storage.
- There are inbuit capabilities which can help you in handling schema drift scenarios.
- This capability is more around Data lake storage where you can do the profiling on the data stored in Data lake stores.


