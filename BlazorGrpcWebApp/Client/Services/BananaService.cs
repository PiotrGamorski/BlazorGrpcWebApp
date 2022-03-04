using BlazorGrpcWebApp.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class BananaService : IBananaService
    {
        private readonly HttpClient _httpClient;
        private readonly IGrpcUserService _grpcUserService;

        // one needs to introduce event Action as StateHasChanged is only avaiable in razor files.
        public event Action? OnChange;
        public int Bananas { get; set; }

        public BananaService(HttpClient httpClient, IGrpcUserService grpcUserService)
        {
            _httpClient = httpClient;
            _grpcUserService = grpcUserService;
        }

        public async Task EatBananas(int amount)
        {
            Bananas -= amount;
            await BananasChanged();
        }

        public async Task AddBananas(int amount)
        {
            var result = await _httpClient.PutAsJsonAsync("api/user/addbananas", amount);
            Bananas = await result.Content.ReadFromJsonAsync<int>();
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

        [Authorize]
        public async Task GrpcGetBananas()
        {
            var userId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            Bananas = (await _grpcUserService.DoGrpcGetUserBananas(new GrpcUserBananasRequest() { UserId = userId })).Bananas;
        }
    }
}
