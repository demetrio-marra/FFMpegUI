using FFMpegUI.Infrastructure.Support;
using Microsoft.Data.SqlClient;
using Polly;

namespace FFMpegUI.Infrastructure.Resilience
{
    public class ResilientPoliciesLocator
    {
        private readonly IAsyncPolicy SqlPolicy;

        public ResilientPoliciesLocator()
        {
            SqlPolicy = BuildSqlPolicy();
        }


        public IAsyncPolicy GetPolicy(ResilientPolicyType policyType)
        {
            switch (policyType)
            {
                case ResilientPolicyType.SqlDatabase: return SqlPolicy;
            }

            throw new NotImplementedException();
        }


        private static IAsyncPolicy BuildSqlPolicy()
        {
            var rnd = new Random();

            var basePolicyBuilder = Policy.Handle<SqlException>(ex => SqlServerTransientErrorDetector.IsTransient(ex));
            var waitAndRetryPolicy = basePolicyBuilder.WaitAndRetryAsync(5, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential back-off
                + +TimeSpan.FromMilliseconds(rnd.Next(0, 5000)) // plus jitter
            );

            return waitAndRetryPolicy;
        }
    }
}
