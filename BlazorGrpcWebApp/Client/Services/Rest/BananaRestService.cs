using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared.Models;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Rest
{
    public class BananaRestService : IBananaRestService
    {
        private readonly HttpClient _httpClient;
        public event Action? OnChange;
        public int Bananas { get; set; }

        public BananaRestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task BananasChanged()
        {
            OnChange?.Invoke();
            return Task.CompletedTask;
        }

        public async Task EatBananas(int amount)
        {
            Bananas -= amount;
            await BananasChanged();
        }

        public async Task<GenericAuthResponse<int>> GetBananas(int authUserId)
        {
            var response = await _httpClient.GetFromJsonAsync<GenericAuthResponse<int>>($"api/user/{authUserId}/getbananas");
            if (response!.Success)
            {
                Bananas = response.Data;
                await BananasChanged();
            }

            return response;
        }

        public async Task AddBananas(int amount)
        {
            var result = await _httpClient.PutAsJsonAsync("api/user/addbananas", amount);
            Bananas = await result.Content.ReadFromJsonAsync<int>();
            
            await BananasChanged();
        }
    }
}
