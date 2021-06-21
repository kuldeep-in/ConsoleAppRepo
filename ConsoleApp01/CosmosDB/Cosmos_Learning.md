### Learning Resources

- https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-get-started
- https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-application
- https://docs.microsoft.com/en-us/azure/cosmos-db/import-data
- https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-sql-api-dotnet-bulk-import
- https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-query-sql-api
- https://docs.microsoft.com/en-us/azure/cosmos-db/create-notebook-visualize-data
- https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-global-distribution-sql-api?tabs=dotnetv2%2Capi-async

### Hands on Lab
- https://azurecosmosdb.github.io/labs/

### Set region in .net code
```
CosmosClient cosmosClient = new CosmosClient(
    "<connection-string-from-portal>", 
    new CosmosClientOptions()
    {
        ApplicationRegion = Regions.WestUS2,
    });
```

### SQL api query
```
SELECT i.name, c.code
from Items i
join c in i.customproperties
```

### Diagnostics Queries (KQL)
```
AzureDiagnostics
| where ResourceProvider=="MICROSOFT.DOCUMENTDB" and Category =="DataPlaneRequests"

AzureActivity 
| where ResourceProvider=="Microsoft.DocumentDb" and Category=="DataPlaneRequests" 
| summarize count() by Resource

AzureActivity 
| where Caller == "test@company.com" and ResourceProvider=="Microsoft.DocumentDb" and Category=="DataPlaneRequests" 
| summarize count() by Resource

# RU Consumed per second
AzureDiagnostics
| where Category == "DataPlaneRequests"
| where collectionName_s == "gaming-001" 
| summarize ConsumedRUsPerSec = sum(todouble(requestCharge_s)) by bin(TimeGenerated, 1s)
| project TimeGenerated, ConsumedRUsPerSec
| render timechart

# Max RU consumed per hour
let T = AzureDiagnostics
| where Category == "DataPlaneRequests"
| where collectionName_s == "gaming-001" 
| summarize ConsumedRUsPerSec = sum(todouble(requestCharge_s)) by bin(TimeGenerated, 1s)
| project TimeGenerated, ConsumedRUsPerSec
| render columnchart     
;
T
| summarize MaxRUPerHour = max(ConsumedRUsPerSec) by bin(TimeGenerated, 1h)
| render timechart

# Top resource consuming operations
AzureDiagnostics
| where collectionName_s == "gaming-001" 
| where databaseName_s == "ms-demo-001"
| order by requestCharge_s desc 
| take 10
| project TimeGenerated,regionName_s, operationType_s, partitionKey_s,requestCharge_s,TenantId

| render piechart 

# Requests caller and ip addresses
AzureActivity
| where ActivityStatus  =~ "Succeeded"
| where ResourceProvider =~ "Microsoft.DocumentDB"
| summarize Count = count() by Caller, CallerIpAddress
| project Caller, CallerIpAddress, Count
| render barchart 

# Activity trend
AzureActivity
| where ActivityStatus  =~ "Succeeded"
| where ResourceProvider =~ "Microsoft.DocumentDB"
| summarize ConsumedRUsPerSec = count() by bin(TimeGenerated, 1m)
| render timechart

# 429-request distribution
AzureDiagnostics
| where collectionName_s == "gaming-001" 
| where statusCode_s == "429"
| summarize Count = count() by OperationName
| project OperationName, Count
| render piechart 

# Total Number of requests per minute:
AzureMetrics
| where MetricName == "TotalRequests"
| project TimeGenerated, Count
| render timechart
```

### Agenda

| Time  | Topics                             |         |
| ----- | ----------------------------------- | ---------------- |
| 10:00 |        |              |
| 10:45 |                    |                  |
| 11:00 |         |              |


