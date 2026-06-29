namespace LocalKYC.Workflow;

public interface IIdSwiftClient
{
    Task<string?> StartKycSessionAsync(string idSwiftEndpoint, SubscriberActivationRequest request, CancellationToken cancellationToken = default);
}
