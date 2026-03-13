using System.Net.Http.Headers;

namespace Client.Infrastructure.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenStorageService _tokenStorage;

    public AuthHeaderHandler(TokenStorageService tokenStorage)
        => _tokenStorage = tokenStorage;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenStorage.AccessToken);

        return base.SendAsync(request, ct);
    }
}