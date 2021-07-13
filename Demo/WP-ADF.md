# WP - ADF

# Azure Data Factory Basic Concepts

## Azure Data Factory
- Azure Data factory is a fully managed data migration and integration service.
-	It is a cloud based ETL and Data integration service, which lets you to create your data driven workflows for orchestrating data movements or for transforming your data at scale.
-	Using Azure Data factory you can create your workflows, which we call as pipelines and you can also trigger them at your scheduled times.
-	Data factory allows you to ingest the data from variety of data sources, which can be either on cloud or on on-premise.
-	On top of this you can published your transformed data sets into data stores. which can be further consumed in BI applications.
-	Some of the key capabilities of the Data Factory are:
    - Flexible Data integration
    - Hybrid Data Orchastration
    - Data movement as a service

## Flexible data integration
-	ADF provides you the capabilities to Modernize your data warehouse, where you can use advanced analytics services like HDInsights or data lake analytics
-	You can Create custom Saas application for your customers, where you can use the programming language of your choice.
-	Here you don’t need to worry about your data scattered at the multiple locations. With the help of the data factory you can bring your data together from various different data sources.


# M03 - Transformation and Control Flow
## M03L01 - Transformation in ADF
### Knowledge Check
- Name a Data Transformation activity?
    - HDInsight, Machine Learning, SQL Server, Data Lake Analytics, Databricks

- What Transformation supports On-Demand Compute Environment?
    - HDInsight Spark

- What Azure service is used to compute a Custom Activity?
    - Azure Batch


## M03L02 - ControlFlow in ADF
### Knowledge Check
- Name one specialized control flow activity?
    - Filter, Execute Pipeline, ForEach, Web, Lookup, Get Metadata, Until, If Condition, and Wait

- Name a purpose for using control flow?
    - Error handling
    - Execution logic based on conditions/values

### Demo : Control Flow
- General
    - Execute Pipeline
    - Get Metadata (Copy -> Get metadata))
    - Lookup (Table dataset)
    - Web
    - Wait
- Iterational and conditional
    - For Each (Lookup -> foreach (storedproc)
    - If Condition
    - Switch
    - unti

# M04 - Integration Runtime
## M04L01 - Integration Runtime - ADF
### Integration Runtime
- The Integration Runtime (IR) is the compute infrastructure used by Azure Data Factory,
- This infrastructure provides data integration capabilities across different network environments.
- There are 3 core capabilities of the integration runtimes:
    - Data movement
    - Pipeline execution
    - And SSIS package execution

### Integration Runetime Types
- There are 3 types of integration runtimes.

### Azure Inntegration Runetime
- First one is Azure Integration runtime, Which provides you serverless computing capabilities in Azure cloud.
- With Azure Integration runtime You don’t have to worry about infrastructure provision, software installation, patching, or capacity scaling. 
- On top of this you only pay for the duration of the actual utilization.
- You can set how many data integration units to use on the copy activity, and the compute size of the Azure IR is elastically scaled up accordingly, without you having to explicitly adjusting size of the Azure Integration Runtime.
- Activity dispatch is a kind of an operation to route the activity to the target compute service, so there is no need to scale up the compute size for this scenario.

### Self Hosted Integration Runtime
- If you would like to perform data integration securely in a private network environment, which does not have a direct line-of-sight from the public cloud environment, you can install a self hosted IR on premises environment behind your corporate firewall, or inside a virtual private network
- For scalability, you can scale out the self-hosted IR by associating the logical instance with multiple on-premises machines in active-active mode.
- As of now Self hosted integration runtimes are only supported on Windows operating system.

### Pre-requisites
- N/A

### Data Flow Self Hosted IR
1. In the first step, you can create a self hosted integration runtime within your data factory using powershell comandlet.
2. Step 2 is creating a linked service using the self hosted integration runtime.
3. In step 3, self hosted integration run time installed on your VMs or in private network stores the credentials required to connect from cloud network.
4. Step 4 is a control channel established between azure data factory and integration runtime installed in private network. Azure Data factory uses this control channel for 
all the communications like scheduling and managing the jobs.
5. And based on the commands from control channel, we can perform read and write 
operation between cloud and private network.

### Usage Consideration Self hosted IR
Here are some of the usage consideration for self hosted integration runtime.
1. You can use a single self hosted integration runtime for multiple data sources across different on premise networks.
2. You can have only one self hosted integration run time for one Azure data factory.
3. In case of multiple machines connected to same one premise data source there can be multiple self hosted integration runtimes on multiple machines

### Usage Consideration Cont'd
- Sometimes integration runtime looks like a gateway, which we usually setup for power bi on premise data sources.
- Don't get confused here, if you already have a gateway setup, still you sed to setup self hosted integration run for Azure Data factory.
- Use Azure integration runtime only for your data stored in public cloud, but if you have setup virtual network for your data store then it is always recommended to use self hosted integration runtime.

### Sharing Self hosted IR
- You can reuse an existing self-hosted integration runtime infrastructure that you may already have setup in a data factory. 
- This allows you to create a linked self-hosted integration runtime in a different data factory by referencing an already existing self-hosted IR (Shared).

### Port and Domain requirements
- N/A

### Limitations - Sharing self hosted IR
Lets see some of the limitations of sharing self hosted integration runtime.
1. One self hosted integration runtime allows 20 linked integration runtimes under it.
2. For sharing your integration runtime, version must be greater than 3.8
3. To grant permissions for inherited integration runtime, user will require owner role or inherited owner role in azure data factory.

### Azure IR Locations
- If you have strict data compliance requirements and need ensure that data do not leave a certain geography, you can explicitly create an Azure IR in a certain region and point the Linked Service to this IR using ConnectVia property. 
- For example, if you want to copy data from Blob in UK South to SQL DW in UK South and want to ensure data do not leave UK, create an Azure IR in UK South and link both Linked Services to this IR

### Azure SSIS IR Location
- In case of Azure SSIS IR, we need to have a SSIS ID hosted on Azure, which can be on Azure SQL Server or Azure managed instance.
- Location of your Azure-SSIS IR does not need be the same as the location of your data factory, but it should be the same as the location of your own Azure SQL Database/Managed Instance server where SSISDB is to be hosted. This way, your Azure-SSIS Integration Runtime can easily access SSISDB without incurring excessive traffics between different locations.
- If you do not have an existing Azure SQL Database/Managed Instance server to host SSISDB, but you have on-premises data sources/destinations, you should create a new Azure SQL Database/Managed Instance server in the same location of a virtual network connected to your on-premises network. This way, you can create your Azure-SSIS IR using the new Azure SQL Database/Managed Instance server and joining that virtual network, all in the same location, effectively minimizing data movements across different locations.
- If the location of your existing Azure SQL Database/Managed Instance server where SSISDB is hosted is not the same as the location of a virtual network connected to your on-premises network, first create your Azure-SSIS IR using an existing Azure SQL Database/Managed Instance server and joining another virtual network in the same location, and then configure a virtual network to virtual network connection between different locations.

### Integration Runtime Location (Diagram)
- The Data Factory location is where the metadata of the data factory is stored and where the triggering of the pipeline is initiated from. Meanwhile, a data factory can access data stores and compute services in other Azure regions to move data between data stores or process data using compute services. 
- This behavior is realized through the globally available IR to ensure data compliance, efficiency, and reduced network egress costs.
- The IR Location defines the location of its back-end compute, and essentially the location where the data movement, activity dispatching, and SSIS package execution are performed. The IR location can be different from the location of the data factory it belongs to.

### Which IR to use
- N/A

### Knowledge Check
- How many instances of self-hosted IR can be installed?
   - 1
- Can Self hosted IR be shared with multiple Data factories? 
   - No

## M04L02 - SSIS in Azure
### SSIS Integration Runtime
- Azure-SSIS IR is a fully managed cluster of Azure VMs dedicated to run your SSIS packages. To run your SSIS packages in Azure, first you need to host them somewhere. To host the SSIS packages, you can either use your Azure SQL database or Azure SQL managed instance server.
- You can scale up the power of the compute by specifying node size and scale it out by specifying the number of nodes in the cluster. You can manage the cost of running your 
Azure-SSIS Integration Runtime by stopping and starting it as you see fit.
- After provisioning your SSISDB, you can use any tool like SSTD or SQL server management studio to deploy your packages and you can simply run them just like SSIS on premise.

### Deploying SSIS IR
- NA

### Prerequisites
- NA

### Azure SSIS integration – Virtual network
- Azure Data Factory lets you join your Azure-SSIS integration runtime to a virtual network created through the classic deployment model or the Azure Resource Manager deployment model.
- If SSIS packages access only public cloud data stores, you don't need to join the Azure-SSIS IR to a virtual network. 
- If SSIS packages access on-premises data stores, you must join the Azure-SSIS IR to a virtual network that is connected to the on-premises network. 

### Virtual network (cont)
- If you join your Azure-SSIS IR to the same virtual network as the Managed Instance, make sure that the Azure-SSIS IR is in a different subnet than the Managed Instance. 
- If you join the Azure-SSIS IR to a different virtual network than the Managed Instance, we recommend either virtual network peering (which is limited to the same region) or a virtual network to virtual network connection
- To deploy your virtual network, you can either use classic deployment method or you can also use ARM templates.

### Virtual network config requirements
- You must be registered to Microsoft Batch resource provider under your subscription.
- Some other network requirements are like having a proper subnet, Domain name server.
- Network security groups can be used to filter the network traffic in an azure virtual network.
- You can use Azure expressrouts or your own user defined routes to establish a connection between Azure resources and on premises servers

### Selecting Subnet–Virtual Network Configuration
- While selecting your subnet, make sure you have enough address spaces in that subnet.
- Just for easy calculation atleast leave double the number of nodes of your integration runtime.
- All azure services have their own subnet range, which you can find on Microsoft documentation. So while selecting your subnet, leave the address already occupied 
by azure services.

### Domain name service server:
- Make sure your SSIS IR can resolve public azure host names, like blob.core.windows.net for azure storage blob, database.windows.net for Azure sql servers.
- It is recommended to forward your requests from custom DNS to Azure DNS, so that Azure DNS can resolve azure host names.
- For your virtual network, it is recommended to setup your custom DNS as primary and Azure DNS as secondary 

### Network Security Group
- For the inbound and outbound connection data factory uses these ports to communicate with the nodes of your Azure SSIS Integration runtime in your virtual network.

### Azure Active Directory configuration
- To setup your azure AD, first create a new group in Azure AD and add Azure data factory manage instance to that group.
- For your Azure SQL databases you need to enable Azure AD authentication.
- And same for your Azure SQL manage instance, you have to enable Azure AD
