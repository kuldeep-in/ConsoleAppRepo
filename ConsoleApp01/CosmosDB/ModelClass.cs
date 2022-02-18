
namespace ConsoleApp01
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public static class ModelClass
    {
        public const string EndpointUrl = "https://{accountname}.documents.azure.com:443/";
        public const string AuthorizationKey = "";

        public static List<string> siteList = new List<string>
                                            {
                                                "onecasinos.com",
                                                "twocasinos.com",
                                                "threecasinos.com",
                                                "fourcasinos.com",
                                                "fivecasinos.com"
                                            };

        public static List<string> categoryList = new List<string>
                                            {
                                                "oneGaming",
                                                "twoGaming",
                                                "threeGaming",
                                                "fourGaming",
                                                "fiveGaming"
                                            };

        public static List<string> typeList = new List<string>
                                            {
                                                "oneStake",
                                                "twoStake",
                                                "threeStake",
                                                "fourStake",
                                                "fiveStake"
                                            };
    }

    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string LastName { get; set; }
        public Parent[] Parents { get; set; }
        public Child[] Children { get; set; }
        public Address Address { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime RegistrationDate { get; set; }
        // The ToString() method is used to format the output, it's used for demo purpose only. It's not required by Azure Cosmos DB
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int Grade { get; set; }
        public Pet[] Pets { get; set; }
    }

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }

    public class balance
    {
        public string walletType { get; set; }
        public string walletName { get; set; }
        public int adjustments { get; set; }
        public double winnings { get; set; }
        public int ringfence { get; set; }
    }

    public class effect
    {
        public string walletType { get; set; }
        public string walletName { get; set; }
        public string compartment { get; set; }
        public Int64 dateTime { get; set; }
        public int amount { get; set; }
    }

    public class gaming
    {
        public Int64 gameId { get; set; }
        public string gameName { get; set; }
        public string gameCode { get; set; }
        public string gamingEventId { get; set; }
        public string productType { get; set; }
        public string status { get; set; }
        public string remoteId { get; set; }
        public string gameEngine { get; set; }

        public action action { get; set; }

    }

    public class action
    {
        public string id { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
    }

    public class LibraryUsers
    {
        [JsonProperty(PropertyName = "id")]
        public string DocumentId { get; set; }

        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "User_First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "User_Last_Name")]
        public string LastName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class GameObject
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string playerId { get; set; }
        public string siteCode { get; set; }
        public Int64 dateTime { get; set; }
        public int ttl { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string reff { get; set; }
        public string remoteRef { get; set; }
        public string correlationToken { get; set; }

        public double totalBalanceAfter { get; set; }
        public gaming gaming { get; set; }
        public effect[] effects { get; set; }
        public balance[] balances { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class GameObject02
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public Int64 playerId { get; set; }
        public string siteCode { get; set; }
        public Int64 dateTime { get; set; }
        public int ttl { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string reff { get; set; }
        public string remoteRef { get; set; }
        public string correlationToken { get; set; }

        public double totalBalanceAfter { get; set; }
        public gaming gaming { get; set; }
        public effect[] effects { get; set; }
        public balance[] balances { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class RecordsCount
    {
        public string playerId { get; set; }
        [JsonProperty(PropertyName = "$1")]
        public int count { get; set; }
    }

    public class Demo03Feb
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "accountid")]
        public int accountid { get; set; }
    }
}
