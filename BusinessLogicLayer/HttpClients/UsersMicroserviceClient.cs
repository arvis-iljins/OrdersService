using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.HttpClients
{
    public class UsersMicroserviceClient(
        HttpClient httpClient,
        ILogger<UsersMicroserviceClient> logger,
        IDistributedCache cache
    )
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<User?> GetUserById(Guid userId)
        {
            HttpResponseMessage response;
            string cacheKey = $"user_{userId}";

            try
            {
                var cachedUser = await _cache.GetStringAsync(cacheKey);
                if (cachedUser is not null)
                {
                    var user = JsonSerializer.Deserialize<User>(cachedUser, JsonOptions);
                    if (user is not null)
                        _logger.LogInformation("User found in cache for userId {UserId}", userId);
                    return user;
                }
                response = await _httpClient.GetAsync($"/api/users/{userId}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "Network error contacting user service for userId {UserId}",
                    userId
                );
                throw new UserServiceUnavailableException("User service is unreachable.", ex);
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    ex,
                    "Request to user service timed out for userId {UserId}",
                    userId
                );
                throw new UserServiceUnavailableException("Request to user service timed out.", ex);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "User service returned {StatusCode} for userId {UserId}. Body: {Body}",
                    response.StatusCode,
                    userId,
                    errorBody
                );
                throw new UserServiceException($"Unexpected status code: {response.StatusCode}");
            }

            try
            {
                User? user = await response.Content.ReadFromJsonAsync<User>(JsonOptions);

                if (user is null)
                    _logger.LogWarning(
                        "User service returned null deserialization result for userId {UserId}",
                        userId
                    );

                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(100))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300));

                _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), cacheOptions);
                _logger.LogInformation("User not found for userId in cache {UserId}", userId);
                return user;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to deserialize user response for userId {UserId}",
                    userId
                );
                throw new UserServiceException("Invalid response format from user service.", ex);
            }
        }
    }

    internal class UserServiceUnavailableException(string? message, Exception? innerException)
        : Exception(message, innerException);

    internal class UserServiceException(string? message, Exception? innerException = null)
        : Exception(message, innerException);
}
