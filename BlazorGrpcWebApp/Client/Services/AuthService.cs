using BlazorGrpcWebApp.Shared;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GenericAuthResponse<string>> Login(UserLogin userLogin)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", userLogin);
#pragma warning disable CS8603 // Possible null reference return.
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<string>>();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<GenericAuthResponse<int>> Register(UserRegister userRegister)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", userRegister);

#pragma warning disable CS8603 // Possible null reference return.
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<int>>();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
