using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace BusinessLogicLayer.Policies
{
    public class UserMicroservicePolicies(ILogger<UserMicroservicePolicies> logger)
        : IUserMicroservicePolicies
    {
        private readonly ILogger<UserMicroservicePolicies> _logger = logger;

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy
                .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(2),
                    onBreak: (outcome, timespan) =>
                    {
                        _logger.LogWarning(
                            $"Circuit braked open for {timespan.TotalSeconds} seconds."
                        );
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit closed. Requests will flow normally.");
                    }
                );

            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 5,
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
