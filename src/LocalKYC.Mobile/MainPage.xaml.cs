using LocalKYC.Workflow;

namespace LocalKYC.Mobile;

public partial class MainPage : ContentPage
{
private readonly SubscriberOnboardingService _subscriberOnboardingService;

public MainPage(SubscriberOnboardingService subscriberOnboardingService)
{
InitializeComponent();
_subscriberOnboardingService = subscriberOnboardingService;
}

private async void OnStartKycClicked(object sender, EventArgs e)
{
if (string.IsNullOrWhiteSpace(FullNameEntry.Text) ||
string.IsNullOrWhiteSpace(NationalIdEntry.Text) ||
string.IsNullOrWhiteSpace(MsisdnEntry.Text) ||
string.IsNullOrWhiteSpace(IdSwiftEndpointEntry.Text))
{
ResultLabel.Text = "Please complete all fields before starting KYC.";
StepsCollectionView.ItemsSource = null;
return;
}

StartButton.IsEnabled = false;
ResultLabel.Text = "Contacting IDSwift...";

try
{
var request = new SubscriberActivationRequest(
FullNameEntry.Text.Trim(),
NationalIdEntry.Text.Trim(),
MsisdnEntry.Text.Trim());

var plan = await _subscriberOnboardingService.StartAsync(IdSwiftEndpointEntry.Text.Trim(), request);

ResultLabel.Text = plan.KycSessionId is null
? "KYC failed. Check IDSwift endpoint and payload format."
: $"KYC session started: {plan.KycSessionId}";

StepsCollectionView.ItemsSource = plan.Steps;
}
finally
{
StartButton.IsEnabled = true;
}
}
}
