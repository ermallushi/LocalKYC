using System.Net.Http.Json;
using System.Text.Json;

namespace LocalKYC.Workflow;

public sealed class IdSwiftClient : IIdSwiftClient
{
    private readonly HttpClient _httpClient;

    public IdSwiftClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> StartKycSessionAsync(string idSwiftEndpoint, SubscriberActivationRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(idSwiftEndpoint, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var payload = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);
        var root = payload.RootElement;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("sessionId", out var sessionId) && sessionId.ValueKind == JsonValueKind.String)
            {
                return sessionId.GetString();
            }

            if (root.TryGetProperty("id", out var id) && id.ValueKind == JsonValueKind.String)
            {
                return id.GetString();
            }
        }

        return null;
    }
}
