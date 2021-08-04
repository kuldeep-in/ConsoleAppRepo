# WP - Cosmos DB

## M03L01 Data Modelling with SQL API

## M03L02 Querying with SQL API

### Demonstration - Querying Cosmos DB

```
SELECT * FROM c
 
SELECT c.name AS FullName FROM c
 
SELECT c.name,c.isActive FROM c
WHERE c.isActive = true
 
SELECT c.balance FROM c

SELECT c.balance,(c.balance<"$1,500.00")?"Low": ((c.balance<"$3,000.00")?"Medium": "High") AS Richness FROM c
  
SELECT c.registered,c.registered< "2015-07-08T08:25:51 +04:00" AS Early
FROM customers c
 
Built-in Functions
SELECT GETCURRENTDATETIME() AS ExecDateTime,IsBool(c.name) AS isNameBool,c.name,c.isActive FROM c
WHERE c.isActive = true
 
SELECT COUNT(c.name) AS TotalNames
FROM c

```










