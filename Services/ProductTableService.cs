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
    }
}
