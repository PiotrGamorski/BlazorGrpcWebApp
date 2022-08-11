using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorGrpcWebApp.Client.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        private readonly HttpClient _httpClient;
        private readonly AuthServicesProvider _servicesProvider;

        public CustomAuthStateProvider(ISessionStorageService sessionStorageService, HttpClient httpClient, 
            AuthServicesProvider customAuthServicesProvider)
        {
            _sessionStorageService = sessionStorageService;
            _httpClient = httpClient;
            _servicesProvider = customAuthServicesProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authToken = await _sessionStorageService.GetItemAsync<string>("authToken");
            var identity = new ClaimsIdentity();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            return await Task.FromResult(state);
        }

        public async Task MarkUserAsAuthenticated(string authToken)
        {
            ClaimsIdentity identity;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }
                catch(Exception e)
                {
                    await _sessionStorageService.RemoveItemAsync("authToken");
                    throw new Exception(e.Message);
                }

                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);
                await ExecuteOnAuthentication(state);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }

        public Task MarkUserAsLoggedOut()
        {
            _sessionStorageService.RemoveItemAsync("authToken");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

            return Task.CompletedTask;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));

            return claims;
        }

        private async Task ExecuteOnAuthentication(AuthenticationState authState)
        {
            await _servicesProvider._bananaService.GrpcGetBananas();
            _servicesProvider._topMenuService.SetProperties(authState);
            _servicesProvider._userRolesService.SetUserRoles(authState);
        }
    }
}
