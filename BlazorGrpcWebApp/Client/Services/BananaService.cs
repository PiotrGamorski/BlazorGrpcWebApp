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

        #region Simplyfing/Helper Methods
        private async Task<int> GetAuthUserId() => await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
        private Task BananasChanged()
        {
            OnChange?.Invoke();
            return Task.CompletedTask;
        }
        #endregion

        public async Task EatBananas(int amount)
        {
            Bananas -= amount;
            await BananasChanged();
        }

        #region REST Api calls
        public async Task GetBananas()
        {
            Bananas = await _httpClient.GetFromJsonAsync<int>("api/user/getbananas");
            await BananasChanged();
        }

        public async Task AddBananas(int amount)
        {
            var result = await _httpClient.PutAsJsonAsync("api/user/addbananas", amount);
            Bananas = await result.Content.ReadFromJsonAsync<int>();
            await BananasChanged();
        }
        #endregion

        #region gRPC calls
        [Authorize]
        public async Task GrpcAddBananas(int amount)
        {
            var result = await _grpcUserService.DoGrpcUserAddBananas(new GrpcUserAddBananasRequest()
            { 
                Amount = amount, 
                UserId = await GetAuthUserId()
            });

            Bananas = result.Bananas;
            await BananasChanged();
        }

        [Authorize]
        public async Task GrpcGetBananas()
        {
            Bananas = (await _grpcUserService.DoGrpcGetUserBananas(new GrpcUserBananasRequest() { UserId = await GetAuthUserId() })).Bananas;
        }
        #endregion
    }
}
