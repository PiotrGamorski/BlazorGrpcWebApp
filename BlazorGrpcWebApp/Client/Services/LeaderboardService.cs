using AutoMapper;
using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    //Delivers functionality to Leaderboard Razor page
    [Authorize]
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly GrpcChannel _channel;
        private UserServiceGrpc.UserServiceGrpcClient _userServiceGrpcClient;
        private BattleLogServiceGrpc.BattleLogServiceGrpcClient _battleLogServiceGrpcClient;
        public IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        public LeaderboardService(IMapper mapper, HttpClient httpClient)
        {
            _mapper = mapper;
            _httpClient = httpClient;
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _userServiceGrpcClient = new UserServiceGrpc.UserServiceGrpcClient(_channel);
            _battleLogServiceGrpcClient = new BattleLogServiceGrpc.BattleLogServiceGrpcClient(_channel);
        }
    

        public async Task GetLeaderboardWithRest()
        {
            var response = await _httpClient.GetFromJsonAsync<IList<UserLeaderboardEntry>>("api/user/leaderboard");
            // TODO: Add if condition?
            foreach (var userLeaderboardEntry in response!)
            {
                var leaderboardItem = _mapper.Map<GrpcUserGetLeaderboardResponse>(userLeaderboardEntry);
                if (!Leaderboard.Contains(leaderboardItem)) 
                    Leaderboard.Add(leaderboardItem);

            }
        }
        //TODO: split this service into Rest and gRPC
        //TODO: add rest methods


        public async Task GetLeaderboardWithGrpc()
        {
            Leaderboard = new List<GrpcUserGetLeaderboardResponse>();
            var response = _userServiceGrpcClient.GrpcUserGetLeaderboard(new GrpcUserGetLeaderboardRequest() { });

            while (await response.ResponseStream.MoveNext(new CancellationToken()))
            {
                if (!Leaderboard.Contains(response.ResponseStream.Current)) 
                    Leaderboard.Add(response.ResponseStream.Current);
            }
        }

        public async Task<bool> ShowBattleLogsWithGrpc(int opponentId)
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var response = await _battleLogServiceGrpcClient.GrpcShowBattleLogsAsync(new GrpcShowBattleLogsRequest
            {
                AuthUserId = authUserId,
                OppenentId = opponentId,
            });

            return response.Show;
        }

        public async Task<List<string>> GetBattleLogsWithGrpc(int opponentId)
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
