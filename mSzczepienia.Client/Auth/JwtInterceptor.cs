using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

namespace mSzczepienia.Client.Auth;

public class JwtInterceptor : DelegatingHandler
{
    private readonly IServiceProvider _services;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<JwtInterceptor> _logger;

    public JwtInterceptor(IServiceProvider services, NavigationManager navigationManager, ILogger<JwtInterceptor> logger)
    {
        _services = services;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var authStateProvider = _services.GetRequiredService<AuthenticationStateProvider>();
            var localStorage = _services.GetRequiredService<ILocalStorageService>();

            var token = await localStorage.GetItemAsync<string>("authToken");

            if (authStateProvider is CustomAuthStateProvider customProvider)
            {
                if (!string.IsNullOrEmpty(token) && customProvider.CheckIfTokenExpired(token))
                {
                    await customProvider.LogoutAsync();
                }
                else
                {
                    _logger.LogWarning("Unauthorized request without valid token, but if token is present it's not expired");
                }
            }

            _navigationManager.NavigateTo("/login");
        }

        return response;
    }
}