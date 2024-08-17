using Azure;
using Azure.Data.Tables;
using static exercise6.class1;

string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=bhava;AccountKey=lZJCvW00yf3nct1dE6b43PpK6AtQcaISE5gi67GlYNHDxZ17wLmV9CnSmWuvNkudq3+ghl+veDbP+AStnpksbQ==;EndpointSuffix=core.windows.net";
// New instance of the TableClient class
TableServiceClient tableServiceClient = new TableServiceClient(ConnectionString);

// New instance of TableClient class referencing the server-side table
TableClient tableClient = tableServiceClient.GetTableClient(
    tableName: "adventureworks"
);

await tableClient.CreateIfNotExistsAsync();

// Create new item using composite key constructor
var prod1 = new Product()
{
    RowKey = "68719518388",
    PartitionKey = "gear-surf-surfboards",
    Name = "Ocean Surfboard",
    Quantity = 8,
    Sale = true
};

// Add new item to server-side table
await tableClient.AddEntityAsync<Product>(prod1);



// Read a single item from container
var product = await tableClient.GetEntityAsync<Product>(
    rowKey: "68719518388",
    partitionKey: "gear-surf-surfboards"
);
Console.WriteLine("Single product:");
Console.WriteLine(product.Value.Name);


// Read multiple items from container
var prod2 = new Product()
{
    RowKey = "68719518390",
    PartitionKey = "gear-surf-surfboards",
    Name = "Sand Surfboard",
    Quantity = 5,
    Sale = false
};

await tableClient.AddEntityAsync<Product>(prod2);

var products = tableClient.Query<Product>(x => x.PartitionKey == "gear-surf-surfboards");

Console.WriteLine("Multiple products:");
foreach (var item in products)
{
    Console.WriteLine(item.Name);
} 

