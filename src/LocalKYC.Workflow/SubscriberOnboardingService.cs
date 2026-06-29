namespace LocalKYC.Workflow;

public sealed class SubscriberOnboardingService
{
    private readonly IIdSwiftClient _idSwiftClient;

    public SubscriberOnboardingService(IIdSwiftClient idSwiftClient)
    {
        _idSwiftClient = idSwiftClient;
    }

    public async Task<OnboardingPlan> StartAsync(string idSwiftEndpoint, SubscriberActivationRequest request, CancellationToken cancellationToken = default)
    {
        var steps = new List<OnboardingStep>();

        try
        {
            var sessionId = await _idSwiftClient.StartKycSessionAsync(idSwiftEndpoint, request, cancellationToken);
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                steps.Add(new OnboardingStep("KYC with IDSwift", OnboardingStepState.Failed, "IDSwift did not return a KYC session id."));
                AddPendingPostKycSteps(steps);
                return new OnboardingPlan(null, steps);
            }

            steps.Add(new OnboardingStep("KYC with IDSwift", OnboardingStepState.Completed, $"KYC session {sessionId} created."));
            steps.Add(new OnboardingStep("CRM Activation", OnboardingStepState.Pending, "Create subscriber profile in CRM and assign SIM."));
            steps.Add(new OnboardingStep("Digital Signature", OnboardingStepState.Pending, "Capture agreement signature and archive consent."));
            steps.Add(new OnboardingStep("SIM Provisioning", OnboardingStepState.Pending, "Push final activation to network provisioning systems."));

            return new OnboardingPlan(sessionId, steps);
        }
        catch (Exception ex)
        {
            steps.Add(new OnboardingStep("KYC with IDSwift", OnboardingStepState.Failed, ex.Message));
            AddPendingPostKycSteps(steps);

            return new OnboardingPlan(null, steps);
        }
    }

    private static void AddPendingPostKycSteps(List<OnboardingStep> steps)
    {
        steps.Add(new OnboardingStep("CRM Activation", OnboardingStepState.Pending, "Run this after KYC is completed."));
        steps.Add(new OnboardingStep("Digital Signature", OnboardingStepState.Pending, "Collect customer signature after CRM activation."));
        steps.Add(new OnboardingStep("SIM Provisioning", OnboardingStepState.Pending, "Finalize profile provisioning on HLR/HSS."));
    }
}
