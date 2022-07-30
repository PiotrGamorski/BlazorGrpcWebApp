using BlazorGrpcWebApp.Client.Interfaces.Grpc;
using BlazorGrpcWebApp.Shared;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Grpc
{
    public class ArmyGrpcService : IArmyGrpcService
    {
        private readonly GrpcChannel _channel;
        private ArmyServiceGrpc.ArmyServiceGrpcClient _armyServiceGrpcClient;
        private readonly HttpClient _httpClient;

        public ArmyGrpcService(HttpClient httpClient)
        {
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _armyServiceGrpcClient = new ArmyServiceGrpc.ArmyServiceGrpcClient(_channel);
            _httpClient = httpClient;
        }

        [Authorize]
        public async Task<GrpcHealUnitResponse> DoGrpcHealUnit(int userUnitId)
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var result = await _armyServiceGrpcClient.GrpcHealUnitGrpcAsync(new GrpcHealUnitRequest()
            {
                AuthUserId = authUserId,
                UserUnitId = userUnitId
            });

            return result;
        }

        [Authorize]
        public async Task<GrpcReviveArmyResponse> DoGrpcReviveArmy()
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var result = await _armyServiceGrpcClient.GrpcReviveArmyAsync(new GrpcReviveArmyRequest() { AuthUserId = authUserId });
            return result;
        }
    }
}
