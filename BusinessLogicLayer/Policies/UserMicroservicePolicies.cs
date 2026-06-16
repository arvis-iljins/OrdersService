using Microsoft.Extensions.Logging;
using Polly;

namespace BusinessLogicLayer.Policies
{
    public class UserMicroservicePolicies(ILogger<UserMicroservicePolicies> logger)
        : IUserMicroservicePolicies
    {
        private readonly ILogger<UserMicroservicePolicies> _logger = logger;

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(
                            "Delaying for {delay} seconds, then making retry {retry}.",
                            timespan.TotalSeconds,
                            retryAttempt
                        );
                    }
                );
        }
    }
}
