using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace ACM_Online_Shopping_App
{
    public class Program
    {
        // The Azure Cosmos DB endpoint for running this sample
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["https://jkays.documents.azure.com:443/"];
        // The primary key for the Azure Cosmos account
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["QOLYNq8FVyzWDcy5WyMrWHWsmOIX0WXJyi5usKkd7bsZxA6iSeuwBfVL9WGTWI7x0WPT27l8JQG1t8N6vWyLMA=="];
        // The Cosmos client instance
        private CosmosClient cosmosClient;
        // The database we will create
        private Database Store;
        //The container we will create
        private Container Shirts;
        private Container Pants;
        private Container Cart;
        // The name of the database and container we will create
        private string databaseId = "Store";
        private string container1Id = "Shirts";
        private string container2Id = "Pants";
        private string container3Id = "Cart";
        bool showMenu = true;
        bool showManagerMenu = false;
        bool showCustomerMenu = false;
        private Container SectionName;
        private string SectionId;
        string DesiredId, NewItemId, NewItemColor, NewItemBrand, NewItemPrice;
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.CreateStoreAsync();
                await p.InitialStoreAsync();
                await p.CreateCartAsync();
                await p.ShoppingMenus();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occured: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of Application, press any key to exit.");
                Console.ReadKey();
            }
        }
        // </main>

        // <GetStartedDemoAsync>
        /// <summary>
        /// Entry point to call methods that operate on Azure Cosmos DB resourcs in this sample
        /// </summary>
        /// <returns></returns>
        public async Task ShoppingMenus()
        {
            //Create A new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            while (showMenu == true)
            {
                showMenu = await LoginMenu();
            }
            while (showManagerMenu == true)
            {
                showManagerMenu = await ManagerMenu();
            }
            while (showCustomerMenu == true)
            {
                showCustomerMenu = await CustomerMenu();
            }
        }
        // </GetStartedDemoAsync>

        //<CreateDatabaseAsync>
        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateStoreAsync()
        {
            //Create a new database
            this.Store = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.Store.Id);
        }
        //</CreateDatabaseAsync>

        //<CreateContainerAsync>
        ///<summary>
        /// Create the container if it does not exist
        /// Specify "/lastname" as the partition key since we're storing family information, to ensure good distribution of requests and storage. 
        ///</summary>
        /// <returns></returns>

        private async Task CreateSectionAsync()
        {
            //Create a new container
            Console.WriteLine("Input a name for the new section");
            var SectionName = Console.ReadLine();
            string SectionId = Console.ReadLine();
            Family NewFamily1  = new Family; 
            this.SectionName = await this.Store.CreateContainerIfNotExistsAsync(SectionId, "/ItemType");
            Console.WriteLine("Created Container: {0}\n", this.SectionName.Id);

        }
        private async Task CreateShirtsAsync()
        {
            this.Shirts = await this.Store.CreateContainerIfNotExistsAsync(container1Id, "/ItemType");
            Console.WriteLine("Created Container: {0}\n", this.Pants.Id);
        }
        private async Task CreatePantsAsync()
        {
            //Create a new container
            this.Pants = await this.Store.CreateContainerIfNotExistsAsync(container2Id, "/ItemType");
            Console.WriteLine("Created Container: {0}\n", this.Pants.Id);
        }
        private async Task CreateCartAsync()
        {
            //Create a new container
            this.Cart = await this.Store.CreateContainerIfNotExistsAsync(container2Id, "/ItemType");
            Console.WriteLine("Created Container: {0}\n", this.Cart.Id);
        }
        //</CreateContainerAsync>
        ///<summary>
        /// Add Clothing Items to the container
        ///</summary

        private async Task InitialStoreAsync()
        {
            await CreateShirtsAsync();
            await CreatePantsAsync();
            //Create a family object for Shirts
            Family shirts = new Family
            {
                Id = "Shirts.1",
                ItemType = "Shirts",
                T_Shirts = new Shirts[]
                {
                    new Shirts { Color = "Olive Green", Brand = "Nike", Price = "$19.99" },
                    new Shirts { Color = "Navy Blue", Brand = "Adidas", Price = "$14.99" }
                },
                Dress_Shirts = new Shirts[]
                {
                    new Shirts
                    {
                        Brand = "Ralph Lauren",
                        Color = "White",
                        Price = "$49.99"
                    }

                },
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> shirtsResponse = await this.Shirts.ReadItemAsync<Family>(shirts.Id, new PartitionKey(shirts.ItemType));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                //Create an item in the container representing the shirts.Note we provide the value of the partition key for this item, which is "Shirts"
                ItemResponse<Family> shirtsResponse = await this.Shirts.CreateItemAsync<Family>(shirts, new PartitionKey(shirts.ItemType));
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", shirtsResponse.Resource.Id, shirtsResponse.RequestCharge);

            }
            //Create a family object for the pants
            Family pants = new Family
            {
                Id = "Pants.1",
                ItemType = "Pants",
                Jeans = new Pants[]
                {
                    new Pants { PantType = "Jeans", Brand = "Levi's", Color = "Dark Blue", Price = "$39.99" },
                    new Pants { PantType = "Jeans", Brand = "Arizona", Color = "Light Blue", Price = "$29.99" }
                },
                Dress_Pants = new Pants[]
                {
                    new Pants { PantType = "Dress_Pants", Brand = "Dockers", Color = "Khaki", Price = "$49.99"}
                },
            };

            try
            {
                //Read the item to see if it exists
                ItemResponse<Family> pantsResponse = await this.Pants.ReadItemAsync<Family>(pants.Id, new PartitionKey(pants.ItemType));
                Console.WriteLine("item in database with id: {0} already exists\n", pantsResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                //Create an item in the container representing the pants
                ItemResponse<Family> pantsResponse = await this.Pants.CreateItemAsync<Family>(shirts, new PartitionKey(pants.ItemType));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", pantsResponse.Resource.Id, pantsResponse.RequestCharge);
            }
        }
        //</AddItemsToContainerAsync>
        private async Task AddItemsToCartAsync()
        {
            Console.WriteLine("What section is your desired item from?");
            string DesiredSection = Console.ReadLine();
            if (DesiredSection == "Shirts")
            {
                Console.WriteLine("What is the name of your desired item?");
                string DesiredId = Console.ReadLine();
                ItemResponse<Family> shirtsResponse = await this.Shirts.ReadItemAsync<Family>(Shirts.DesiredId, new PartitionKey(Shirts.ItemType));
                ItemResponse<Family> CartResponse = await this.Cart.CreateItemAsync<Family>(Shirts, new PartitionKey(Shirts.ItemType));
                Console.WriteLine("Item {0} added to cart\n", CartResponse.Resource.Id);
            }
            else if (DesiredSection == "Pants")
            {
                Console.WriteLine("What is the name of your desired item?");
                string DesiredId = Console.ReadLine();
                ItemResponse<Family> PantsResponse = await this.Pants.ReadItemAsync<Family>(Shirts.DesiredId, new PartitionKey(Pants.ItemType));
                ItemResponse<Family> CartResponse = await this.Cart.CreateItemAsync<Family>(Shirts, new PartitionKey(Pants.ItemType));
                Console.WriteLine("Item {0} added to cart\n", CartResponse.Resource.Id);
            }
            else
            {
                Console.WriteLine("That is not a valid Section");
            }
            
        }
        private async Task AddItemsToStoreAsync()
        {
            Console.WriteLine("What section would you like to add to?");
            string DesiredSection = Console.ReadLine();
            if (DesiredSection == "Shirts")
            {
                Console.WriteLine("What is the name of the new item?");
                string NewItemId = Console.ReadLine();
                Console.WriteLine("What is the color of the new item?");
                string NewItemColor = Console.ReadLine();
                Console.WriteLine("What is the brand of the new item?");
                string NewItemBrand = Console.ReadLine();
                Console.WriteLine("What is the price of the new item?");
                string NewItemPrice = Console.ReadLine();
                try
                {
                    ItemResponse<Family> ShirtsResponse = await this.Shirts.ReadItemAsync<Family>(Shirts.NewItemId, new PartitionKey(Shirts.ItemType));
                }
                catch
                {
                    ItemResponse<Family> ShirtsResponse = await this.Cart.CreateItemAsync<Family>(Shirts, new PartitionKey(Shirts.ItemType));
                    Console.WriteLine("Item {0} added to store\n", ShirtsResponse.Resource.Id);
                }
            }
            else if (DesiredSection == "Pants")
            {
                Console.WriteLine("What is the name of your desired item?");
                Console.WriteLine("What is the name of the new item?");
                string NewItemId = Console.ReadLine();
                Console.WriteLine("What is the color of the new item?");
                string NewItemColor = Console.ReadLine();
                Console.WriteLine("What is the brand of the new item?");
                string NewItemBrand = Console.ReadLine();
                Console.WriteLine("What is the price of the new item?");
                string NewItemPrice = Console.ReadLine();
                try
                {
                    ItemResponse<Family> PantsResponse = await this.Pants.ReadItemAsync<Family>(Shirts.DesiredId, new PartitionKey(Pants.ItemType));
                }
                catch
                {
                    ItemResponse<Family> PantsResponse = await this.Cart.CreateItemAsync<Family>(Shirts, new PartitionKey(Pants.ItemType));
                    Console.WriteLine("Item {0} added to store\n", PantsResponse.Resource.Id);
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Section");
            }
        }

        //<QueryItemsAsync>
        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the contianer
        /// </summary>
        private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.ItemType = 'shirts'";

            Console.WriteLine("Running query {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Family> queryResultSetIterator = this.Shirts.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> ItemTypes = new List<Family>();
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    ItemTypes.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }
        // </QueryItemsAsync>

        // <ReplaceFamilyItemAsync>
        /// <summary>
        /// Replace an item in the container
        /// </summary>
        
        private async Task ReplaceItemAsync()
        {
            ItemResponse<Family> pantsResponse = await this.Shirts.ReadItemAsync<Family>("Pants.7", new PartitionKey("Pants"));
            var itemBody = pantsResponse.Resource;

            //update color and brand
            itemBody.Jeans[0].Color = "Black";
            itemBody.Jeans[0].Brand = "Guess";
            // replace the item with the updated content
            pantsResponse = await this.Shirts.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.ItemType));
            Console.WriteLine("Updated Family [{0}, {1}].\n\tBody is now: {2}\n", itemBody.ItemType, itemBody.Id, pantsResponse.Resource);
        }
        //</ReplaceFamilyItemAsync>

        //<DeleteFamilyItemAsync>
        /// <summary>
        /// Delete an item in the container
        /// </summary>
        
        private async Task DeleteItemAsync()
        {
            var partitionKeyValue = "pants";
            var familyId = "pants.7";

            var famliyId = "pants.7";
            // Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<Family> pantsResponse = await this.Shirts.DeleteItemAsync<Family>(famliyId, new PartitionKey(partitionKeyValue));
            Console.WriteLine("Deleted Family [{0}, {1}]\n", partitionKeyValue, familyId);
        }
        // </DeleteFamilyItemAsync>
        
        // <DeleteDatabaseAndCleanupAsync>
        /// <summary>
        /// Delete the database and dispose o fthe Cosmos Client instance
        /// </summary>
        
        private async Task DeleteDatabaseAndCleanupAsync ()
        {
            DatabaseResponse databaseResourceResponse = await this.Store.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();
            Console.WriteLine("Deleted Database: {0]\n", this.databaseId);
            //Dispose of CosmosClient
            this.cosmosClient.Dispose();
        }
        // </DeleteDatabaseAndCleanupAsync>
        private async Task<bool> LoginMenu()
        {
            Console.Clear();
            Console.WriteLine("Are you a Manager or a Customer?");
            Console.WriteLine("1. Manager");
            Console.WriteLine("2. Customer");
            Console.WriteLine("3. Exit");
            Console.WriteLine("\nSelect an option by its number: ");

            switch (Console.ReadLine())
            {
                case "1":
                    showMenu = false;
                    showManagerMenu = true;
                    await ManagerMenu();
                    return true;
                case "2":
                    showMenu = false;
                    showCustomerMenu = true;
                    await CustomerMenu();
                    return true;
                case "3":
                    return false;
                default:
                    return true;
            }
        }
        private async Task<bool> ManagerMenu()
        {
            Console.Clear();
            Console.WriteLine("What is the password for managers?");
            if (Console.ReadLine() == "Management")
            {
                Console.Clear();
                Console.WriteLine("What would you like to do to the store?");
                Console.WriteLine("1. Create New Section (shirts, pants, etc.)");
                Console.WriteLine("2. Add items");
                Console.WriteLine("3. Replace Items");
                Console.WriteLine("4. Delete Items");
                Console.WriteLine("5. Delete Database");
                Console.WriteLine("6. Exit and Return to Login Menu");
                Console.WriteLine("\nSelect an option by its number: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        await this.CreateSectionAsync();
                        showManagerMenu = true;
                        return true;
                    case "2":
                        await this.AddItemsToStoreAsync();
                        showManagerMenu = true;
                        return true;
                    case "3":
                        await this.ReplaceItemAsync();
                        showManagerMenu = true;
                        return true;
                    case "4":
                        await this.DeleteItemAsync();
                        showManagerMenu = true;
                        return true;
                    case "5":
                        await this.DeleteDatabaseAndCleanupAsync();
                        showManagerMenu = true;
                        return true;
                    case "6":
                        showManagerMenu = false;
                        showMenu = true;
                        return false;
                    default:
                        showManagerMenu = true;
                        return true;
                }
            } else
            {
                Console.WriteLine("That is the incorrect password, you will be directed back to the Login Menu");
                showManagerMenu = false;
                showMenu = true;
                return false;
            }
        }
        private async Task<bool> CustomerMenu()
        {
            Console.Clear();
            Console.WriteLine("What would you like to do in the store?");
            Console.WriteLine("1. Add Items to Cart");
            Console.WriteLine("2. Remove Items from Cart");
            Console.WriteLine("3. Exit");
            Console.WriteLine("\nSelect an option by its number: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await this.QueryItemsAsync();
                    await this.AddItemsToCartAsync();
                    showCustomerMenu = true;
                    return true;
                case "2":
                    await this.DeleteItemAsync();
                    showCustomerMenu = true;
                    return true;
                case "3":
                    showCustomerMenu = false;
                    showMenu = true;
                    return false;
                default:
                    showCustomerMenu = true;
                    return true;
            }
        }
    }
}

