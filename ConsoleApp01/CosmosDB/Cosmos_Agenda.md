# 2 Days Workshop

### Day 1
- Architecture overview.
- Consistency levels
    - Consistence, availability and performance tradeoffs.
- How to query using SQL
    - [Demo]
    - [Lab]
- Creation of stored procedures and triggers and UDFs.
    - [Demo]
    - [Labs]
- Partitioning, key choice, horizonal scaling.
- Data Modeling
- Pricing
- Change feeds.
    - Set up change feeds
    - [Demo]

### Day 2 
- Performance Monitoring.
    - Azure monitor
    - Setup cosmos metrics in Azure monitor
    - Query tuning
    - [Demo]
- Backup and recovery.
- Migrating data to CosmosDB
    - Data migration tool to migrate data to CosmosDB
    - [Demo]
- Security
- Database Throughput?
    - Throughput for database and containers.
    - Distribution based on partition key
    - [Demo]

# RAW
```
Day 1 (focused on engineering and architectural design)
· High level architecture view.
· Eventually consistent levels and how this equates to MongoDB. Implications of the different levels.
	o Consistence, availability and performance tradeoffs.
· How to query using SQL
	o [Demo]
	o [Lab]
· Creation of stored procedures and triggers and UDFs.
	o [Demo]
	o [Labs]
· Partitioning, key choice, horizonal scaling.
· Design considerations, modeling, best practice.
· Pricing
· Limitations
· RTO using Multi-master.
	o Multi region write
	o Conflicts resolution
· Change feeds.
	o Set up change feeds
	o [Demo]
 
Day 2 (focused on operational topics) 
· How to monitor performance, optimize and identifying bad queries and tuning of queries.
	o Azure monitor
	o Setup cosmos metrics in Azure monitor
	o Query tuning
	o [Demo]
· Backup solution and recovery. 
	o How cosmos db maintain backups.
	o Various recovery options
· Maintenance tasks and offline tasks.
· Cosmos DB is fully managed database service, what exactly we are looking to achieve by tasks ?
· Migrating data to CosmosDB
	o Data migration tool to migrate data to CosmosDB
	o [Demo]
· Security, permissions, roles etc.
	o Overview of security permission and roles
· How does it scale, can we do it elastically?
	o Throughput for database and containers.
	o Distribution based on partition key
	o Various ways to scale the database.
	o [Demo]
· Common operational problems, pitfalls.
	o Finding right value of throughput for your database.
```
