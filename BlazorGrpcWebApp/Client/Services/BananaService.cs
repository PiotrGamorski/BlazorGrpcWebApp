using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Client.Interfaces.Grpc;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class BananaService : IBananaService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserGrpcService _grpcUserService;

        // one needs to introduce event Action as StateHasChanged is only avaiable in razor files.
        public event Action? OnChange;
        public int Bananas { get; set; }

        public BananaService(HttpClient httpClient, IUserGrpcService grpcUserService)
        {
            _httpClient = httpClient;
            _grpcUserService = grpcUserService;
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

        #region REST Api calls
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
        #endregion

        #region Helper Methods for gRPC Calls
        private async Task<int> GetAuthUserId() => await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
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
