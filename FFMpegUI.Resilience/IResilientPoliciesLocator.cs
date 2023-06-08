using Polly;

namespace FFMpegUI.Resilience
{
    public interface IResilientPoliciesLocator
    {
        IAsyncPolicy GetPolicy(ResilientPolicyType policyType);
    }
}