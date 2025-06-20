using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductFunction.Helpers;
using ProductFunction.Services;
using System.Net;
using System.Text.Json;

namespace ProductFunction.Functions
{
    public class GetProductsFunction
    {
        private readonly ILogger<GetProductsFunction> _logger;
        private readonly IProductService _productService;

        public GetProductsFunction(ILogger<GetProductsFunction> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [Function("GetProducts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                // Henter alle produkter
                var products = await _productService.GetProductsAsync();

                // 200 OK respons med produktene serialisert til JSON
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(JsonSerializer.Serialize(products));
                return response;
            }
            catch (System.Exception ex)
            {
                // Logger feil ved henting
                _logger.LogError(ex, "Feil ved henting av produkter");

                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync(ErrorMessages.FetchError);
                return errorResponse;
            }
        }
    }
}
