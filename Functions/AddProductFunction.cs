using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductFunction.Helpers;
using ProductFunction.Models;
using ProductFunction.Services;
using System.Net;
using System.Text.Json;

namespace ProductFunction.Functions
{
    public class AddProductFunction
    {
        private readonly ILogger<AddProductFunction> _logger;
        private readonly IProductService _productService;

        public AddProductFunction(ILogger<AddProductFunction> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [Function("AddProduct")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                //  Leser request body 
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var product = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Sjekker om deserialiseringen og data er gyldige
                if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0 || string.IsNullOrEmpty(product.Category))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync(ErrorMessages.InvalidData);
                    return badResponse;
                }

                await _productService.AddProductAsync(product);

                // respons 201 på at produktet er lagret i tabell
                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteStringAsync("Produkt lagret.");
                return response;
            }
            catch (Exception ex)
            {
                // Logger eventuelle feil som oppstår 
                _logger.LogError(ex, "Feil ved lagring av produkt");

                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync(ErrorMessages.InternalError);
                return errorResponse;
            }
        }
    }
}
