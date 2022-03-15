using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;
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
        public LeaderboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _userServiceGrpcClient = new UserServiceGrpc.UserServiceGrpcClient(_channel);
        }

        public IList<UserStatistic>? Leaderboard { get; set; } = new List<UserStatistic>();
        public IList<GrpcUserGetLeaderboardResponse> GrpcLeaderboardResponses { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        public async Task GetLeaderboardRestApi()
        {
            Leaderboard = await _httpClient.GetFromJsonAsync<IList<UserStatistic>>("api/user/leaderboard");
        }

        public async Task DoGrpcGetLeaderboard()
        {
            var response = _userServiceGrpcClient.GrpcUserGetLeaderboard(new GrpcUserGetLeaderboardRequest() { });
            while (await response.ResponseStream.MoveNext(new CancellationToken()))
            { 
                if(!GrpcLeaderboardResponses.Contains(response.ResponseStream.Current))
                    GrpcLeaderboardResponses.Add(response.ResponseStream.Current);
            }
        }
    }
}
