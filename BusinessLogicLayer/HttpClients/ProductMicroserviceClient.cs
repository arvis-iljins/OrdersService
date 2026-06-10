using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.HttpClients
{
    public class ProductMicroserviceClient(
        HttpClient httpClient,
        ILogger<ProductMicroserviceClient> logger
    )
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ProductMicroserviceClient> _logger = logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<ProductDTO?> GetProductById(int productId)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync($"/api/products/{productId}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "Network error contacting product service for productId {ProductId}",
                    productId
                );
                throw new ProductServiceUnavailableException("Product service is unreachable.", ex);
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    ex,
                    "Request to product service timed out for productId {ProductId}",
                    productId
                );
                throw new ProductServiceUnavailableException(
                    "Request to product service timed out.",
                    ex
                );
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Product service returned {StatusCode} for productId {ProductId}. Body: {Body}",
                    response.StatusCode,
                    productId,
                    errorBody
                );
                throw new ProductServiceException($"Unexpected status code: {response.StatusCode}");
            }

            try
            {
                ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>(
                    JsonOptions
                );

                if (product is null)
                    _logger.LogWarning(
                        "Product service returned null deserialization result for productId {ProductId}",
                        productId
                    );

                return product;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to deserialize product response for productId {ProductId}",
                    productId
                );
                throw new ProductServiceException(
                    "Invalid response format from product service.",
                    ex
                );
            }
        }
    }

    internal class ProductServiceUnavailableException(string? message, Exception? innerException)
        : Exception(message, innerException);

    internal class ProductServiceException(string? message, Exception? innerException = null)
        : Exception(message, innerException);
}
