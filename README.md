# Serverless - Functions & Logic Apps

## Agenda
-	capabilities and benefits of the serverless computing model.
-	Azure functions, which is a serverless service available in Azure.
-	use cases where you can use functions.
-	Then we will dive into technical bit, where we will talk about function triggers, input and output bindings.
-	We will see, how we can configure the functions.
-	At the end of the session we will talk about the Azure Logic apps.

## What is Serverless
-	Lets first understand what is serverless model.
-	When I say serverless means, you don’t need to worry about provisioning and managing servers. You don’t need to think of how much resources your service will consume.
-	Serverless is full abstraction of servers, where your developers can simply focus on their code without worrying about infrastructure.
-	Instant -event driven scalability.
-	Serverless applications, can react to the events and triggers associated with them. They can be scaled when you need more processing. 
-	Pay-as you go.
-	As you are not provisioning any servers or infrastructure, so the billing is calculated on the number times you will call your application or how long your application was running and how much memory was used.

## What are the benefits:
Some of the benefits of using serverless computing model are.
-	You can simply focus on your business problem, without worrying about technology and how your application will run.
-	We all want a cost-effective platform to host our applications, so serverless helps you in moving from fixed cost model to the variable cost model.
-	And this will lead to less waste of resources
-	Its super easy to start with functions, that you will see in a while during the demonstration.
-	You gets the flexibility to test or experiment with your code.

## Azure Ecosystem integration
-	How you can leverage the capabilities of Azure to build your serverless applications.
-	If I talk about the development
    - Then you can start writing your function code using your favourite IDEs.
    - You can store your source code in devops repositories.
    - You can develop and test your functions in local environment.
    - You can monitor or debug your functions runs and even you can view the history of past executions.
-	On the platform front
    - We have got event grid, which can be used to build event based architecture applications. 
    - Event grid has built-in support for events coming from Azure services like storage accounts or databases.
    - Azure functions and logic apps are another serverless platform, which would be the focus of today’s session.

## What is Azure functions:
-	 Azure function is an event based serverless compute experience that accelerate your application development
-	In Azure function you gets built-in triggers, which lets you to define or configure when you would like to invoke your function. 
-	Function provides binding to different data sources, which can be used to get the data from the data stores or you can even pass the data from azure functions to these data stores.
-	You can develop your functions using your favourite IDE or you can also leverage the capability of Azure portal where you can build, test and debug your functions.
-	You can host your functions under existing app service plans or pay as you go option.
-	You can choose from the available options based on your business needs
-	Azure functions are build on top of Function as a service. Where you will get these mentioned features with additional capabilities which will help you in reducing development time and boost your productivity.

## Focus on code, not plumbing
-	There is lot of code you are saving here.
-	Code to connect to services, code for trigger data, code for input and output operations. 
-	In function programming model everything will be taken care by the platform.
-	So you don’t need to worry about the infrastructure, you can run and scale your applications when in need without wasting your resources.

## Boost development efficiency:
-	So If I summarize all the capabilities of azure functions, which increase your development efficiency without worrying about infrastructure are:
-	 Triggers which invokes the functions
-	You can create endpoints as reverse proxies to other apis
-	Leverage the capabilities of DevOps and deploy your functions using CI CD pipelines.
-	With the help of binds you can interact with the different data sources.
-	You can debug or monitor your functions from your favourite IDEs or using developer tools from azure portal

## Azure functions:
- NA

## Deploy your way
- NA

## Azure function use cases
- NA

## Function Triggers
-	Triggers are what causes a function to run.
-	Function must have only one trigger, but can have multiple bindings.

## Input and output bindings
-	Binding means connecting another resource to function.
-	Input binding can be used to create connections to the data stores, from where you are looking to read the data.
-	You can use output binding to return the data to other services, where you can consume or store your data.

## Configuring triggers and bindings
-	You can define your triggers and bindings based on your development approach.
-	Triggers and bindings can be configured by 2 ways.
-	If you are using a C#  class library, you can use decorating methods and parameters.
-	In all other cases, triggers and binds can be configured using simple JSON object.
-	If you will look at the shown JSON snipet, this defines a function which is triggered by HTTP and accept a http playload as input binding.
-	It has 2 output bindings one returning http and other returning a message to the queue.
-	You can also define the data types in the JSON, for the dynamically typed languages like javascript. There are 3 options available for data type, Binary , string and stream. So you can use as per your inuput and output data.

## Authorization – Host Key
-	There are 2 ways of authorizing an Azure function. One of them is using host key.
-	You can find host keys per function app, which can let you to access all the functions created under that function app. 
-	it can’t be revoked but can be renewed
## Authorization – Function Key
-	another way of authorizing your functions is using function key. 
-	Function keys are Per individual function and You can have multiple of them

## Authorization Levels:
-	There are 4 authorization levels that you can define for your functions.
-	…
-	…
-	In your local development environment, all the functions are treated as anonymous authorization level.


## Logic App:
-	Logic app is an Azure cloud service which helps you in 
    - scheduling, 
    - automating and 
    - orchestrating tasks, business processes or your workflows.
-	With the help of logic apps you can design and build scalable solutions for 
    - app integration and 
    - data integration weather in cloud or in on premises or both.
-	You can integrate machine learning or cognitive service applications with the help of logic apps.
-	Some of the common use cases can be, sending email notifications with Microsoft 365 event, or moving files from FTP storage to Azure storage.


## Logic App – Diagram
-	Logic apps provides you code free environment, where you can use 200+ connectors to interact with services running in cloud or on-premise.
-	….

## Workflow:
-	….. 

## Triggers:
-	Logic apps support a wide range of triggers, which can be fired automatically or manually.
-	These are some of the common triggers used in logic apps.

## Actions:
Action can be placed in 3 categories:
-	Actions which invokes services
-	Actions which controls the behaviour
-	Actions which handles the messages.
-	Some common examples of actions can be, like deleting an entry from table, or wait for some specific time interval.

## Demo



