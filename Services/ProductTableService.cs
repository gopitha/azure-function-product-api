using ProductFunction.Models;
using Azure.Data.Tables;

namespace ProductFunction.Services
{
    public class ProductTableService : IProductService
    {
        private readonly TableClient _tableClient;

        public ProductTableService()
        {
            //connection string til Azure Storage
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            _tableClient = new TableClient(connectionString, "ProductTable");
            _tableClient.CreateIfNotExists();
        }

        // metode for å legge til nytt produkt i tabellen
        public async Task AddProductAsync(Product product)
        {

            var entity = new TableEntity("Product", Guid.NewGuid().ToString())
            {
                ["Name"] = product.Name,  
                ["Price"] = product.Price,    
                ["Category"] = product.Category     
            };

            await _tableClient.AddEntityAsync(entity);
        }

        //  metode for å hente alle produkter fra tabellen
        public async Task<List<Product>> GetProductsAsync()
        {
            // Henter alle entiteter (rader) som tilhører partition key "Product"
            var entities = _tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Product'").ToList();

            // Gjør om radene til en liste med Product-objekter
            var products = entities.Select(e => new Product
            {
                Name = e.GetString("Name"),
                Price = e.GetInt32("Price") ?? 0,
                Category = e.GetString("Category")
            }).ToList();

            return await Task.FromResult(products);
        }
    }
}
