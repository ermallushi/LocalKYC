namespace LocalKYC.Workflow;

public enum OnboardingStepState
{
    Pending,
    Completed,
    Failed
}

public sealed record SubscriberActivationRequest(
    string FullName,
    string NationalId,
    string Msisdn);

public sealed record OnboardingStep(
    string Name,
    OnboardingStepState State,
    string Notes);

public sealed record OnboardingPlan(
    string? KycSessionId,
    IReadOnlyList<OnboardingStep> Steps);
