using BlazorGrpcWebApp.Client.Interfaces.Rest;
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

        public async Task<GenericAuthResponse<int>?> Register(UserRegister userRegister)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", userRegister);
            return await result.Content.ReadFromJsonAsync<GenericAuthResponse<int>>();
        }
    }
}
