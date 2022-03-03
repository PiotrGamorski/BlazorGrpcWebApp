using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class BananaService : IBananaService
    {
        private readonly HttpClient _httpClient;
        // one needs to introduce event Action as StateHasChanged is only avaiable in razor files.
        public event Action? OnChange;
        public int Bananas { get; set; } = 1000;

        public BananaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task EatBananas(int amount)
        {
            Bananas -= amount;
            await BananasChanged();
        }

        public async Task AddBananas(int amount)
        {
            Bananas += amount;
            await BananasChanged();
        }

        Task BananasChanged()
        { 
            OnChange?.Invoke();
            return Task.CompletedTask;
        }

        public async Task GetBananas()
        {
            Bananas = await _httpClient.GetFromJsonAsync<int>("api/user/getbananas");
            await BananasChanged();
        }
    }
}
