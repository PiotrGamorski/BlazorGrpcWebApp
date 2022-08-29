using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Rest
{
    //Delivers functionality for Login and Register Razor pages
    public class AuthRestService : IAuthRestService
    {
        private readonly HttpClient _httpClient;

        public AuthRestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GenericAuthResponse<string>?> Login(UserLogin userLogin)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", userLogin);
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<string>>();
        }

        public async Task<GenericAuthResponse<int>?> Register(UserRegisterRequestDto request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<int>>();
        }

        public async Task<GenericAuthResponse<object>> Verify(VerifyCodeRequestDto request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/verify", request);
            #pragma warning disable CS8603 // Possible null reference return.
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<object>>();
            #pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
