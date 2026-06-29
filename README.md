# LocalKYC

A starter mobile onboarding project for telecom new-subscriber activation.

## Flow implemented
1. **KYC with IDSwift** (`http://localhost:8086/...` in Docker)
2. **CRM Activation** (queued after successful KYC)
3. **Digital Signature** (queued after CRM)
4. **SIM Provisioning** (extra step to complete activation)

## Visual Studio 2026 usage
Open `/home/runner/work/LocalKYC/LocalKYC/LocalKYC.slnx` in Visual Studio 2026 (with .NET MAUI workload) and run `LocalKYC.Mobile`.

The app main screen allows entering:
- IDSwift endpoint URL (default: `http://localhost:8086/api/v1/kyc/sessions`)
- Full name
- National ID
- MSISDN

When you tap **Start KYC Step**, the app posts data to IDSwift and displays the step-by-step onboarding plan.

## Project structure
- `src/LocalKYC.Mobile` - .NET MAUI mobile app
- `src/LocalKYC.Workflow` - onboarding flow and IDSwift integration service
- `tests/LocalKYC.Workflow.Tests` - focused unit tests for the flow logic

## Run tests
```bash
dotnet test /home/runner/work/LocalKYC/LocalKYC/tests/LocalKYC.Workflow.Tests/LocalKYC.Workflow.Tests.csproj
```
