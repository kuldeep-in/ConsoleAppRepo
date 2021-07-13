# WP - ADF

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
