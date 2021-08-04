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
Families collection
```
SELECT * from f where f.id = "AndersenFamily"

SELECT {"Name":f.id, "City":f.address.city, "State":f.address.state
} AS Family 
FROM Families f

SELECT c.givenName, c.gender FROM c IN f.children

select * from Families.address.zip

SELECT * FROM Families.children[1]

SELECT f.id AS Name,{ "state": f.address.state, "city": f.address.city } AS Address, f.address.zip FROM Families f

SELECT f.id, f.address.state = "CA" AS IsFromCAState FROM Families f

SELECT {"ParentName":p.givenName, "ChildName":c.givenName} AS Name
FROM Families f 
JOIN c IN f.children 
JOIN p IN f.parents

SELECT * 
FROM c IN Families.children
WHERE c.grade > 2

SELECT 
    f.id AS familyName,
    c.givenName AS childGivenName,
    c.firstName AS childFirstName,
    p.givenName AS petName 
FROM Families f 
JOIN c IN f.children 
JOIN p IN c.pets
```








