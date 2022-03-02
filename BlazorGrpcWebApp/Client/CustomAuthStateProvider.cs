using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorGrpcWebApp.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        private readonly HttpClient _httpClient;
        public CustomAuthStateProvider(ISessionStorageService sessionStorageService, HttpClient httpClient)
        {
            _sessionStorageService = sessionStorageService;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // it'll be null at first, but after refresh it'll have the item
            var authToken = await _sessionStorageService.GetItemAsync<string>("authToken");
            ClaimsIdentity identity;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
            else 
                identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            return await Task.FromResult(state);
        }

        public Task MarkUserAsAuthenticated(string authToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }

            return Task.CompletedTask;
        }

        public Task MarkUserAsLoggedOut()
        {
            _sessionStorageService.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;

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
    }
}
