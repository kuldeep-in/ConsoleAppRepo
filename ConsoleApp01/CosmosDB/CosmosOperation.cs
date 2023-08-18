
namespace ConsoleApp01
{
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Scripts;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    class CosmosOperation
    {
        #region variable declare
        private const string EndpointUrl = "https://cosmos-ms-sqlapi.documents.azure.com:443/";
        private const string PrimaryKey = "";

        private const string DatabaseId = "ms-demo-001";
        private const string ContainerName = "DemoCon-01";
        private const string PartitionKey = "/id";

        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
        #endregion

        public async Task RunDemoAsync01()
        {
            try
            {

                await this.CreateCosmosClient();
                await this.CreateDatabaseAsync();
                await this.CreateContainerAsync();

                await this.GenerateDataAsync();

                //Console.WriteLine("Beginning operations...\n");
                //CosmosOperation p = new CosmosOperation();

                //Console.WriteLine("1-Read, 2-Delete, 3-Create");
                //var ip = Console.ReadLine();

                //await p.CreateCosmosClient();
                //if (ip == "1")
                //{
                //    Console.WriteLine("Reading....");
                //    await p.ReadData();
                //}
                //else if (ip == "2") // 2-Delete
                //{
                //    Console.WriteLine("Deleting....");
                //    await p.DeleteItemFromContainer();
                //}
                //else if (ip == "3") // 3-Create
                //{
                //    Console.WriteLine("Creating....");
                //    await p.GenerateGameDataAsync();
                //}

                //await p.DeleteItemFromContainer();
                //await p.StartChangeFeedProcessorAsync();
                //await p.GenerateGameDataAsync();
                //await p.CreateDatawithIntId();
                //await p.ExecuteSP();


                // List<int> intlist = Enumerable.Range(1, 10).ToList();

                //Parallel.ForEach(intlist, i =>
                //{
                //    for (int pl = 1; pl <= 500; pl++)
                //    {
                //        intlist.Add(pl);
                //        Console.WriteLine("##" + pl.ToString() + "##################################");
                //        await p.QueryWithOneFilter();
                //        await p.QueryWithTwoFilters();
                //        await p.GenerateGameDataAsync();
                //        Console.WriteLine("##################################");

                //    }

                //});

                Console.WriteLine("End....");
                Console.Read();


            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }

        }

        private async Task RunDemoAsync()
        {
            //Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
            //DocumentCollection collection = await GetOrCreateCollectionAsync(databaseId, collectionId);

            //Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            //await CreateDocuments(collectionUri);

            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);

            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
            //await this.QueryItemsAsync();

            //--------------------------------------------------------------------------------------------------------
            // There are three ways of writing queries in the .NET SDK for CosmosDB, 
            // using the SQL Query Grammar, using LINQ Provider with Query and with Lambda. 
            // This sample will show how to write SQL Query Grammar using asynchronous requests.
            // This sample will also show how to display request charges for throughput analysis and estimation.
            //--------------------------------------------------------------------------------------------------------

            // Querying for equality using a single filter
            await QueryWithOneFilter();

            // Querying for equality using a double filter
            //await QueryWithTwoFilters();

            // Querying using range operators like >, <, >=, <=
            await QueryWithRangeOperatorsDateTimes();

            // Query a single join
            await QueryWithSingleJoin();

            // Query with a double join
            await QueryWithDoubleJoin();

            // Uncomment to Cleanup
            // await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
        }

        #region DB_Container_Operation
        private async Task CreateCosmosClient()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            Console.WriteLine("Created Database: {0}\n", DatabaseId);
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
            Console.WriteLine("Created Container: {0}\n", ContainerName);
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            Console.WriteLine("Created Database: {0}\n", DatabaseId);
        }

        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
            Console.WriteLine("Created Container: {0}\n", ContainerName);
        }
        #endregion

        #region Queries_And_Operations
        private async Task CreateDatawithIntId()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);

            await this.CreateDatabaseAsync();
            this.container = await this.database.CreateContainerIfNotExistsAsync("Gaming01", "/playerId");
            //this.container02 = await this.database.CreateContainerIfNotExistsAsync(containerId02, "/playerId");

            //Console.WriteLine("Simple query equality. Find family where id = 'AndersenFamily'");
            //Console.WriteLine("SELECT * FROM Families f WHERE f.id = 'AndersenFamily'");
            Console.WriteLine();

            // Query using a single filter on id
            var sqlQueryText = "select c.playerId, count (1)from c group by c.playerId";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<RecordsCount> queryResultSetIterator = this.container.GetItemQueryIterator<RecordsCount>(queryDefinition);

            List<RecordsCount> players = new List<RecordsCount>();

            var options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            Random random = new System.Random();

            try
            {
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<RecordsCount> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    Parallel.ForEach(currentResultSet, options, player =>
                    {
                        //players.Add(player);
                        Console.WriteLine("PlayerId: {0}, RecordCount: {1}", player.playerId, player.count);


                        Int64 intplayerid = Convert.ToInt64(player.playerId);

                        for (int dc = 1; dc <= player.count; dc++)
                        {

                            GameObject02 gameob = new GameObject02
                            {
                                Id = Guid.NewGuid().ToString(),
                                playerId = intplayerid,
                                //"874713150186983439",
                                siteCode = "casinos.com",
                                dateTime = DateTime.UtcNow.Ticks,
                                ttl = 31536000,
                                category = "Gaming",
                                type = "Stake",
                                amount = 4,
                                currency = "GBP",
                                reff = DateTime.UtcNow.Ticks.ToString(),
                                remoteRef = "9034147670",
                                correlationToken = Guid.NewGuid().ToString(),
                                totalBalanceAfter = 626.41,
                                gaming = new gaming
                                {
                                    gameId = DateTime.UtcNow.Ticks,
                                    gameName = "Book of Ra Temple of Gold",
                                    gameCode = "10881",
                                    gamingEventId = DateTime.UtcNow.Ticks.ToString(),
                                    productType = "Casino",
                                    status = "Created",
                                    remoteId = "7588808616",
                                    gameEngine = "Novomatic",
                                    action = new action
                                    {
                                        id = "758516252223728860",
                                        status = "Success",
                                        createdAt = "2019-08-12T14:31:37"
                                    }
                                },
                                effects = new effect[]
                                {
                                    new effect {
                                        walletType = "Cash" ,
                                        walletName = "NonCredit",
                                        compartment = "Winning",
                                        dateTime = DateTime.UtcNow.Ticks,
                                        amount = -4
                                    }
                                },
                                balances = new balance[]
                                {
                                        new balance {
                                            walletType = "Cash",
                                            walletName = "NonCredit",
                                            adjustments = 0,
                                            winnings = 626.41,
                                            ringfence = 0
                                        }
                                }
                            };

                            try
                            {
                                //this.container02.CreateItemAsync<GameObject02>(gameob, new PartitionKey(gameob.playerId));

                                Console.WriteLine("Player: {0}, Document {1}, playerId: {2} ", intplayerid, dc, intplayerid);
                            }
                            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                            {
                                Console.WriteLine("ERROR Player: {0}, Document {1} ", intplayerid, dc);
                            }
                        }


                    });



                    //    foreach (RecordsCount player in currentResultSet)
                    //{
                    //    players.Add(player);
                    //    Console.WriteLine("PlayerId: {0}, RecordCount: {1}", player.playerId, player.count);
                    //}
                    //Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("ERROR Player:");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR Player:");
            }
            //////////////

            // foreach (string player in players)
            //{

            //sqlQueryText = "SELECT DISTINCT (c.playerId) FROM c";
            //QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            //FeedIterator<GameObject> queryResultSetIterator = this.container.GetItemQueryIterator<GameObject>(queryDefinition);

            //List<string> players = new List<string>();

            //while (queryResultSetIterator.HasMoreResults)
            //{
            //    FeedResponse<GameObject> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            //    foreach (GameObject player in currentResultSet)
            //    {
            //        players.Add(player.playerId);
            //        Console.WriteLine("\tRead {0}", player.playerId);
            //    }
            //    //Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            //}
            // }
            Console.WriteLine("Press enter key to continue********************************************");
            Console.ReadKey();
            Console.WriteLine();
        }

        private async Task GenerateDataAsync()
        {
            int playerCount = 10;
            int doccount = 100;
            Random random = new System.Random();

            //var options = new ParallelOptions() { MaxDegreeOfParallelism = 2 };

            //string a = new BigInteger(bytes).ToString();
            //Int64 rplayerid = random.Next();

            //Parallel.For(1, 301, options, pl =>
            for (int pl = 1; pl <= playerCount; pl++)
            {
                //var rng = new RNGCryptoServiceProvider();
                //byte[] bytes = new byte[6];
                //rng.GetBytes(bytes);
                //string strplid = new BigInteger(bytes).ToString();
                string stringPlayerId = DateTime.UtcNow.AddDays(-1).Ticks.ToString();

                for (int dc = 1; dc <= doccount; dc++)
                {
                    string app1 = random.Next(9999, 99999999).ToString();
                    int randint = random.Next(99, 9999);
                    int r = random.Next(ModelClass.siteList.Count());


                    GameObject gameob = new GameObject
                    {
                        id = Guid.NewGuid().ToString(),
                        playerId = stringPlayerId,
                        siteCode = ModelClass.siteList[r],
                        dateTime = DateTime.UtcNow.Ticks,
                        ttl = 31536000,
                        category = ModelClass.categoryList[r],
                        type = ModelClass.typeList[r],
                        amount = randint,
                        currency = "GBP",
                        reff = DateTime.UtcNow.Ticks.ToString(),
                        remoteRef = "90341476" + app1.Substring(3, 2),
                        correlationToken = Guid.NewGuid().ToString(),
                        totalBalanceAfter = randint + 231,
                        gaming = new gaming
                        {
                            gameId = DateTime.UtcNow.Ticks,
                            gameName = "Book of Ra Temple of Gold" + app1.Substring(3, 2),
                            gameCode = "10881" + app1.Substring(3, 2),
                            gamingEventId = DateTime.UtcNow.Ticks.ToString(),
                            productType = "Casino",
                            status = "Created" + app1.Substring(3, 1),
                            remoteId = "7588808616",
                            gameEngine = "Novomatic",
                            action = new action
                            {
                                id = app1,
                                status = "Success" + app1.Substring(3, 1),
                                createdAt = DateTime.UtcNow.ToString()
                            }
                        },
                        effects = new effect[]
                        {
                            new effect {
                                walletType = "Cash" +app1.Substring(1, 1),
                                walletName = "NonCredit",
                                compartment = "Winning",
                                dateTime = DateTime.UtcNow.Ticks,
                                amount = -4
                            }
                        },
                        balances = new balance[]
                        {
                            new balance {
                                walletType = "Cash"+app1.Substring(1, 1),
                                walletName = "NonCredit"+app1.Substring(3, 2),
                                adjustments = 0,
                                winnings = 626.41,
                                ringfence = 0
                            }
                        }
                    };

                    try
                    {
                        await this.container.CreateItemAsync<GameObject>(gameob, new PartitionKey(gameob.id));

                        Console.WriteLine("Player: {0}, Document {1}, playerId: {2} ", pl, dc, gameob.playerId);

                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                    {
                        Console.WriteLine("ERROR Player: {0}, Document {1} ", pl, dc);
                    }
                    finally
                    {
                        Thread.Sleep(2000);
                    }
                }//);
            }

        }

        private async Task AddItemsToContainerAsync()
        {
            // Create a family object for the Andersen family
            Family andersenFamily = new Family
            {
                Id = "Andersen",
                LastName = "Andersen",
                Parents = new Parent[]
                {
           new Parent { FirstName = "Thomas" },
           new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
           new Child
            {
                FirstName = "Henriette Thaulow",
                Gender = "female",
                Grade = 5,
                Pets = new Pet[]
                {
                    new Pet { GivenName = "Fluffy" }
                }
            }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = true,
                RegistrationDate = DateTime.UtcNow.AddDays(-1)
            };

            try
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen".
                ItemResponse<Family> andersenFamilyResponse = await this.container.CreateItemAsync<Family>(andersenFamily, new PartitionKey(andersenFamily.LastName));
                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamily.Id);
            }

            Family wakefieldFamily = new Family
            {
                Id = "WakefieldFamily",
                LastName = "Wakefield",
                Parents = new[] {
                    new Parent { FamilyName= "Wakefield", FirstName= "Robin" },
                    new Parent { FamilyName= "Miller", FirstName= "Ben" }
                },
                Children = new Child[] {
                    new Child
                    {
                        FamilyName= "Merriam",
                        FirstName= "Jesse",
                        Gender= "female",
                        Grade= 8,
                        Pets= new Pet[] {
                            new Pet { GivenName= "Goofy" },
                            new Pet { GivenName= "Shadow" }
                        }
                    },
                    new Child
                    {
                        FirstName= "Lisa",
                        Gender= "female",
                        Grade= 1
                    }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = false,
                RegistrationDate = DateTime.UtcNow.AddDays(-30)
            };

            try
            {
                // Create an item in the container representing the wakefield family. Note we provide the value of the partition key for this item, which is "Andersen".
                ItemResponse<Family> wakefieldFamilyResponse = await this.container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.LastName));
                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Resource.Id, wakefieldFamilyResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamily.Id);
            }

        }

        //get data (multiple queries)
        private async Task ReadData()
        {
            Program p = new Program();
            var sqlQueryText = "select c.playerId, count (1)from c group by c.playerId";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<RecordsCount> queryResultSetIterator = this.container.GetItemQueryIterator<RecordsCount>(queryDefinition);

            List<RecordsCount> players = new List<RecordsCount>();
            List<RecordsCount> playersoutput = new List<RecordsCount>();

            var options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            Random random = new System.Random();
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<RecordsCount> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                Parallel.ForEach(currentResultSet, options, player =>
                {
                    if (player.count > 49)
                    {
                        Console.WriteLine("PlayerId: {0}, RecordCount: {1}", player.playerId, player.count);
                        try
                        {
                            playersoutput.Add(new RecordsCount() { playerId = player.playerId, count = player.count });
                        }
                        catch
                        {
                        }
                    }
                    Int64 intplayerid = Convert.ToInt64(player.playerId);
                });
            }

            //Parallel.ForEach(playersoutput, options, player =>
            //{
            //    if (player.count > 400)
            //    {
            //        Console.WriteLine("PlayerId: {0}, RecordCount: {1}", player.playerId, player.count);
            //        try
            //        {
            //            await this.QueryWithPK();
            //        }
            //        catch
            //        {
            //        }
            //    }
            int r = random.Next(0, playersoutput.Count - 500);

            for (int pl = r; pl <= r + 499; pl++)
            {
                //intlist.Add(pl);
                Console.WriteLine(string.Format("## 01 ({0} / {1}) ######################", pl, r + 499));
                await this.QueryWithPK(playersoutput[pl].playerId);
                await this.QueryWithOneFilter();
                await this.QueryWithTwoFilters(playersoutput[pl].playerId);
                //await p.GenerateGameDataAsync();

                Console.WriteLine("##################################");
                Thread.Sleep(1000);

            }

            r = random.Next(0, playersoutput.Count - 500);

            for (int pl = r; pl <= r + 499; pl++)
            {
                //intlist.Add(pl);
                Console.WriteLine(string.Format("## 02 ({0} / {1}) ######################", pl, r + 499));
                await this.QueryWithPK(playersoutput[pl].playerId);
                await this.QueryWithOneFilter();
                await this.QueryWithTwoFilters(playersoutput[pl].playerId);
                //await p.GenerateGameDataAsync();

                Console.WriteLine("##################################");
                Thread.Sleep(1000);

            }

            r = random.Next(0, playersoutput.Count - 500);

            for (int pl = r; pl <= r + 499; pl++)
            {
                //intlist.Add(pl);
                Console.WriteLine(string.Format("## 03 ({0} / {1}) ######################", pl, r + 499));
                await this.QueryWithPK(playersoutput[pl].playerId);
                await this.QueryWithOneFilter();
                await this.QueryWithTwoFilters(playersoutput[pl].playerId);
                //await p.GenerateGameDataAsync();

                Console.WriteLine("##################################");
                Thread.Sleep(1000);

            }
        }

        //delete
        private async Task DeleteItemFromContainer()
        {
            Random random = new System.Random();
            int startamt = random.Next(99, 9999);
            int endamt = startamt + 50;
            var sqlQueryText = string.Format("select top 3000 * from players p where p.amount > {0} and p.amount < {1}", startamt.ToString(), endamt.ToString());
            Console.WriteLine(sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<GameObject> queryResultSetIterator = this.container.GetItemQueryIterator<GameObject>(queryDefinition);

            List<GameObject> gameobjects = new List<GameObject>();

            while (queryResultSetIterator.HasMoreResults)
            {
                int cnt = 1;
                FeedResponse<GameObject> response = await queryResultSetIterator.ReadNextAsync();
                Console.WriteLine("Result Count: {0}", response.Count());
                foreach (var item in response)
                {
                    Console.WriteLine(string.Format("#{0}/{1} Deleting doc Id:{2}, pID:{3}",
                        cnt.ToString(),
                        response.Count().ToString(),
                        item.id.ToString(),
                        item.playerId.ToString()));
                    try
                    {
                        cnt++;
                        await container.DeleteItemAsync<GameObject>(item.id.ToString(), new PartitionKey(item.playerId.ToString()));
                    }
                    catch
                    {
                        Console.WriteLine("Delete Failed");
                    }
                    Console.WriteLine("Sleeping...");
                    Thread.Sleep(3000);

                }

            }
        }

        //queries
        private async Task QueryWithPK(string playerId)
        {
            Random random = new System.Random();
            //int r = random.Next(ModelClass.playerIdList.Count());

            var sqlQueryText = string.Format("SELECT p.playerId FROM players p WHERE p.playerId = '{0}'", playerId); //ModelClass.playerIdList[r]);
            Console.WriteLine(sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                Console.WriteLine("Result Count: {0}, Request Charge: {1}", currentResultSet.Count(), currentResultSet.RequestCharge);
                //Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }
            Console.WriteLine();
        }

        private async Task QueryWithOneFilter()
        {
            Random random = new System.Random();
            int r = random.Next(ModelClass.siteList.Count());

            // Query using a single filter on id
            //var sqlQueryText = "select top 2000 p.playerId, p.category, p.totalBalanceAfter from players p where p.playerId = '637113135855088557'";
            var sqlQueryText = string.Format("select top 2500 p.playerId from players p where p.category = '{0}'", ModelClass.categoryList[r]);
            Console.WriteLine(sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<GameObject> queryResultSetIterator = this.container.GetItemQueryIterator<GameObject>(queryDefinition);

            List<GameObject> gameobjects = new List<GameObject>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<GameObject> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                Console.WriteLine("Result Count: {0}, Request Charge: {1}", currentResultSet.Count(), currentResultSet.RequestCharge);
                //Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }
            Console.WriteLine();
        }

        private async Task QueryWithTwoFilters(string playerId)
        {
            Random random = new System.Random();
            int r = random.Next(ModelClass.siteList.Count());
            //int r1 = random.Next(ModelClass.playerIdList.Count());

            // Query using a double filter on id and city
            var sqlQueryText = string.Format("SELECT top 2500 p.playerId FROM players p WHERE p.playerId = '{1}' OR p.category = '{0}'", ModelClass.categoryList[r], playerId);
            Console.WriteLine(sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            //List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                Console.WriteLine("Result Count: {0}, Request Charge: {1}", currentResultSet.Count(), currentResultSet.RequestCharge);
                //Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }
            Console.WriteLine();
        }

        private async Task QueryWithRangeOperatorsDateTimes()
        {
            Console.WriteLine("Query using a range operator on a date time");

            // Query using a range operator on a datetime.
            var sqlQueryText = string.Format("SELECT * FROM c WHERE c.RegistrationDate >= '{0}'",
                DateTime.UtcNow.AddDays(-3).ToString("o"));
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
                Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }

            Console.WriteLine("Press enter key to continue********************************************");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private async Task QueryWithSingleJoin()
        {
            Console.WriteLine("Query using a single join on Families and Children");
            Console.WriteLine("SELECT f.id \n" +
                "\tFROM Families f \n" +
                "\tJOIN c IN f.Children");
            Console.WriteLine();

            // Query using a single join on families and children
            var sqlQueryText = "SELECT f.id, c.FirstName AS child " +
                "FROM Families f " +
                "JOIN c IN f.Children";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
                Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }

            Console.WriteLine("Press enter key to continue********************************************");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private async Task QueryWithDoubleJoin()
        {
            Console.WriteLine("Query using a double join on Families, Children, and Pet");
            Console.WriteLine("SELECT f.id as family, c.FirstName AS child, p.GivenName AS pet \n" +
                "\tFROM Families f \n" +
                "\tJOIN c IN f.Children \n" +
                "\tJOIN p IN c.Pets ");

            // Query using a single join on families and children
            var sqlQueryText = "SELECT f.id as family, c.FirstName AS child, p.GivenName AS pet " +
                "FROM Families f " +
                "JOIN c IN f.Children " +
                "JOIN p IN c.Pets ";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
                Console.WriteLine("Request Charge: {0}", currentResultSet.RequestCharge);
            }

        }
        #endregion

        #region StoredProc
        public async Task CreateExecuteSP()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);

            string storedProcedureId = "spCreatecustomer";
            StoredProcedureResponse storedProcedureResponse = await cosmosClient.GetContainer(DatabaseId, "CustomerContainer").Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties
            {
                Id = storedProcedureId,
                Body = File.ReadAllText($@"D:\Source-ms\Repos\01POCRepo\CoreAppSln\ConsoleAppCosmos\spCreatecustomer.js")
            });

            Console.WriteLine(storedProcedureResponse.RequestCharge);

            dynamic[] newItems = new dynamic[]
                {
                    new {
                        //id = "111",
                        customerid = "cus01",
                        orderid = "100",
                        description = "Order 100 for customer 1"
                    },
                    new {
                       // id= "12121",
                        customerid = "cus01",
                        orderid = "101",
                        description = "order 101 for customer 01"
                    }
                };

            var result = await cosmosClient.GetContainer(DatabaseId, "CustomerContainer").Scripts.ExecuteStoredProcedureAsync<string>("spCreatecustomer", new PartitionKey("cus01"), new[] { newItems });
            Console.WriteLine(result.RequestCharge);

        }

        public async Task ExecuteSP()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);

            var result = await cosmosClient.GetContainer("ms-dataportal", "demo-Families")
                            .Scripts.ExecuteStoredProcedureAsync<string>(
                                                                    "demo01", 
                                                                    new PartitionKey("family001"), 
                                                                    new[] { "family001", "true" }
                                                                    );
            Console.WriteLine(result.RequestCharge);

        }
        #endregion

        #region resolutionPolicy
        private async Task DemoLWW()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            Container demo26container = await cosmosClient.GetDatabase("databasename")
                .CreateContainerIfNotExistsAsync(new ContainerProperties("containername", "partitionkey")
                {
                    ConflictResolutionPolicy = new ConflictResolutionPolicy()
                    {
                        Mode = ConflictResolutionMode.LastWriterWins,
                        ResolutionPath = "/mycustomtimestamp"
                    }
                });
        }

        private async Task DemoCustomPolicy()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            Container demo26container = await cosmosClient.GetDatabase("databasename")
                .CreateContainerIfNotExistsAsync(new ContainerProperties("containername", "partitionkey")
                {
                    ConflictResolutionPolicy = new ConflictResolutionPolicy()
                    {
                        Mode = ConflictResolutionMode.Custom,
                        ResolutionProcedure = "dbs/<databaseName>/colls/<collectionname>/sprocs/<storedprocedurename>"
                    }
                });

            await demo26container.Scripts.CreateStoredProcedureAsync(
                new StoredProcedureProperties("procedurename", File.ReadAllText("procedurename.js")));
        }
        #endregion

        #region changefeedpreocessor
        private async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync()
        {
            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey);
            string databaseName = "ms-demo-001";
            string sourceContainerName = "demo03Feb";
            string leaseContainerName = "demo03Feb-leases";

            Container leaseContainer = cosmosClient.GetContainer(databaseName, leaseContainerName);

            ChangeFeedProcessor changeFeedProcessor = cosmosClient.GetContainer(databaseName, sourceContainerName)
                .GetChangeFeedProcessorBuilder<Demo03Feb>(processorName: "changeFeedSample", HandleChangesAsync)
                    .WithInstanceName("consoleHost")
                    .WithLeaseContainer(leaseContainer)
                    .Build();
            Console.WriteLine("Starting Change Feed Processor...");
            await changeFeedProcessor.StartAsync();
            Console.WriteLine("Change Feed Processor started.");
            return changeFeedProcessor;
        }

        static async Task HandleChangesAsync(IReadOnlyCollection<Demo03Feb> changes, CancellationToken cancellationToken)
        {
            Console.WriteLine("Started handling changes...");
            foreach (Demo03Feb item in changes)
            {
                Console.WriteLine($"Detected operation for item with id {item.id}.");
                // Simulate some asynchronous operation
                await Task.Delay(10);
            }
            Console.WriteLine("Finished handling changes.");
        }
        #endregion

        #region Bulk insert
       
        public async Task AddDateCosmosDBAsync(string jsonText, string dataDomainName)
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, PrimaryKey, options);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
            //Container container = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties { Id = dataDomainName, PartitionKeyPath = "/id" });

            Stopwatch stopwatch = Stopwatch.StartNew();
            
            JsonDocument jsonDocument = JsonDocument.Parse(jsonText); 
            JsonElement element = jsonDocument.RootElement; 

            var elements = element.EnumerateArray(); 
            int count = 0;
            List<Task> tasks = new List<Task>();

            while (elements.MoveNext())
            {
                var doc = elements.Current; 
                if (count != 0) 
                {
                    Console.WriteLine(doc);
                    var id = doc.GetProperty("id").ToString(); 
                    var raw = doc.GetRawText(); 
                    byte[] bytes = Encoding.UTF8.GetBytes(raw);
                    using (var stream = new MemoryStream(bytes))
                    {
                        tasks.Add(container.CreateItemStreamAsync(stream, new PartitionKey(id))
                               .ContinueWith((Task<ResponseMessage> task) =>
                               {
                                   using (ResponseMessage response = task.Result)
                                   {
                                       if (!response.IsSuccessStatusCode)
                                       {
                                           Console.WriteLine($"Received {response.StatusCode} ({response.ErrorMessage}).");
                                       }

                                   }
                               })); 
                    }
                }
                count++;
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();
            Console.WriteLine($"Finished in writing {count} items in {stopwatch.Elapsed}.");
        }
        #endregion

        #region otherMethods
        private static void LogException(Exception e)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Exception baseException = e.GetBaseException();
            if (e is CosmosException)
            {
                CosmosException de = (CosmosException)e;
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            else
            {
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

            Console.ForegroundColor = color;
        }

        private static void Assert(string message, bool condition)
        {
            if (!condition)
            {
                throw new ApplicationException(message);
            }
        }

        private static void AssertSequenceEqual(string message, List<Family> list1, List<Family> list2)
        {
            if (!string.Join(",", list1.Select(family => family.Id).ToArray()).Equals(
                string.Join(",", list1.Select(family => family.Id).ToArray())))
            {
                throw new ApplicationException(message);
            }
        }

        string GenerateAuthToken(string verb, string resourceType, string resourceId, string date, string key, string keyType, string tokenVersion)
        {
            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };

            verb = verb ?? "";
            resourceType = resourceType ?? "";
            resourceId = resourceId ?? "";

            string payLoad = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n",
                    verb.ToLowerInvariant(),
                    resourceType.ToLowerInvariant(),
                    resourceId,
                    date.ToLowerInvariant(),
                    ""
            );

            byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
            string signature = Convert.ToBase64String(hashPayLoad);

            return System.Web.HttpUtility.UrlEncode(String.Format(System.Globalization.CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}",
                keyType,
                tokenVersion,
                signature));
        }
        #endregion
    }
}
