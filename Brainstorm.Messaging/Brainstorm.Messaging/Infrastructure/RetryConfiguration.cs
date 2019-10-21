using System;
using Brainstorm.Messaging.Infrastructure.Metadata;
using Azure = Microsoft.Azure.ServiceBus;
using BSIRetryExponential = Brainstorm.Messaging.Infrastructure.RetryExponential;

namespace Brainstorm.Messaging.Infrastructure
{
    internal static class RetryConfiguration
    {
        public static Azure.RetryPolicy Create(RetryStrategy strategy)
        {
            switch (strategy.RetryPolicy.RetryType)
            {
                case RetryType.Default:
                   return Azure.RetryPolicy.Default;
                case RetryType.NoRetry:
                   return Azure.RetryPolicy.NoRetry;
                case RetryType.Exponential:
                   var expPolicyConfig = (BSIRetryExponential)strategy.RetryPolicy;
                   return new Azure.RetryExponential(expPolicyConfig.MinimumBackoff, expPolicyConfig.MaximumBackoff, expPolicyConfig.MaximumRetryCount);
                default:
                   throw new NotSupportedException();
            }
        }
    }
}
