using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    [Authorize]
    public class LeaderboardService : ILeaderboardService
    {
        private readonly HttpClient _httpClient;
        private readonly GrpcChannel _channel;
        private UserServiceGrpc.UserServiceGrpcClient _userServiceGrpcClient;
        private BattleLogServiceGrpc.BattleLogServiceGrpcClient _battleLogServiceGrpcClient;
        public IList<UserStatistic>? Leaderboard { get; set; } = new List<UserStatistic>();
        public IList<GrpcUserGetLeaderboardResponse> GrpcLeaderboardResponses { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        public LeaderboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _userServiceGrpcClient = new UserServiceGrpc.UserServiceGrpcClient(_channel);
            _battleLogServiceGrpcClient = new BattleLogServiceGrpc.BattleLogServiceGrpcClient(_channel);
        }
    

        public async Task GetLeaderboardRestApi()
        {
            Leaderboard = await _httpClient.GetFromJsonAsync<IList<UserStatistic>>("api/user/leaderboard");
        }


        public async Task DoGrpcGetLeaderboard()
        {
            GrpcLeaderboardResponses = new List<GrpcUserGetLeaderboardResponse>();
            var response = _userServiceGrpcClient.GrpcUserGetLeaderboard(new GrpcUserGetLeaderboardRequest() { });
            while (await response.ResponseStream.MoveNext(new CancellationToken()))
            {
                if (!GrpcLeaderboardResponses.Contains(response.ResponseStream.Current))
                {
                    GrpcLeaderboardResponses.Add(response.ResponseStream.Current);
                }
            }
        }

        public async Task<bool> DoGrpcShowBattleLogs(int opponentId)
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var response = await _battleLogServiceGrpcClient.GrpcShowBattleLogsAsync(new GrpcShowBattleLogsRequest
            {
                AuthUserId = authUserId,
                OppenentId = opponentId,
            });

            return response.Show;
        }

        public async Task<List<string>> DoGrpcGetBattleLogs(int opponentId)
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var response = _battleLogServiceGrpcClient.GrpcGetBattleLogs(new GrpcGetBattleLogsRequest()
            {
                AuthUserId = authUserId,
                OppenentId = opponentId
            });

            List<string> BattleLogs = new List<string>();
            while (await response.ResponseStream.MoveNext())
            {
                BattleLogs.Add(response.ResponseStream.Current.Log);
            }

            return BattleLogs;
        }
    }
}
