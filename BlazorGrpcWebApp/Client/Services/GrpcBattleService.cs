using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class GrpcBattleService : IGrpcBattleService
    {
        private readonly GrpcChannel _channel;
        private BattleServiceGrpc.BattleServiceGrpcClient _battleServiceGrpcClient;
        private readonly HttpClient _httpClient;
        public GrpcBattleService(HttpClient httpClient)
        {
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _battleServiceGrpcClient = new BattleServiceGrpc.BattleServiceGrpcClient(_channel);
            _httpClient = httpClient;
        }

        [Authorize]
        public async Task DoGrpcStartBattle(int opponentId)
        {
            try
            {
                var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
                await _battleServiceGrpcClient.GrpcStartBattleAsync(new GrpcStartBattleRequest()
                {
                    AuthUserId = authUserId,
                    OppenentId = opponentId,
                });
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                throw new RpcException(e.Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
