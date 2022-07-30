using BlazorGrpcWebApp.Client.Interfaces.Providers.Rest;
using BlazorGrpcWebApp.Shared.Models;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Providers.Rest
{
    public class AuthRestProvider : IAuthRestProvider
    {
        private readonly HttpClient _httpClient;

        public AuthRestProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GenericAuthResponse<string>?> Login(UserLogin userLogin)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", userLogin);
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<string>>();
        }

        public async Task<GenericAuthResponse<int>?> Register(UserRegister userRegister)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", userRegister);
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<int>>();
        }
    }
}
