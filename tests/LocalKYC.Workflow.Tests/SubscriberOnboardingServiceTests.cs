using LocalKYC.Workflow;

namespace LocalKYC.Workflow.Tests;

public class SubscriberOnboardingServiceTests
{
    [Fact]
    public async Task StartAsync_WhenIdSwiftReturnsSession_CompletesFirstStep()
    {
        var service = new SubscriberOnboardingService(new FakeIdSwiftClient("session-123"));
        var request = new SubscriberActivationRequest("Test User", "A123", "+35561111111");

        var plan = await service.StartAsync("http://localhost:8086/api/v1/kyc/sessions", request);

        Assert.Equal("session-123", plan.KycSessionId);
        Assert.Equal(OnboardingStepState.Completed, plan.Steps[0].State);
        Assert.Equal("CRM Activation", plan.Steps[1].Name);
        Assert.Equal(OnboardingStepState.Pending, plan.Steps[1].State);
        Assert.Equal("Digital Signature", plan.Steps[2].Name);
        Assert.Equal("SIM Provisioning", plan.Steps[3].Name);
    }

    [Fact]
    public async Task StartAsync_WhenIdSwiftFails_ReturnsFailedKycStep()
    {
        var service = new SubscriberOnboardingService(new FakeIdSwiftClient(null));
        var request = new SubscriberActivationRequest("Test User", "A123", "+35561111111");

        var plan = await service.StartAsync("http://localhost:8086/api/v1/kyc/sessions", request);

        Assert.Null(plan.KycSessionId);
        Assert.Equal(OnboardingStepState.Failed, plan.Steps[0].State);
        Assert.All(plan.Steps.Skip(1), step => Assert.Equal(OnboardingStepState.Pending, step.State));
    }

    [Fact]
    public async Task StartAsync_WhenIdSwiftThrows_ReturnsFailureDetails()
    {
        var service = new SubscriberOnboardingService(new ThrowingIdSwiftClient());
        var request = new SubscriberActivationRequest("Test User", "A123", "+35561111111");

        var plan = await service.StartAsync("http://localhost:8086/api/v1/kyc/sessions", request);

        Assert.Null(plan.KycSessionId);
        Assert.Equal(OnboardingStepState.Failed, plan.Steps[0].State);
        Assert.Contains("boom", plan.Steps[0].Notes);
    }

    private sealed class FakeIdSwiftClient : IIdSwiftClient
    {
        private readonly string? _sessionId;

        public FakeIdSwiftClient(string? sessionId)
        {
            _sessionId = sessionId;
        }

        public Task<string?> StartKycSessionAsync(string idSwiftEndpoint, SubscriberActivationRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_sessionId);
        }
    }

    private sealed class ThrowingIdSwiftClient : IIdSwiftClient
    {
        public Task<string?> StartKycSessionAsync(string idSwiftEndpoint, SubscriberActivationRequest request, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("boom");
        }
    }
}
