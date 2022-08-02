using AutoMapper;
using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Rest
{
    //Delivers functionality to Leaderboard Razor page
    [Authorize]
    public class LeaderboardRestService : ILeaderboardRestService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        public LeaderboardRestService(IMapper mapper, HttpClient httpClient)
        {
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task GetLeaderboardWithRest()
        {
            var response = await _httpClient.GetFromJsonAsync<IList<UserLeaderboardEntry>>("api/user/leaderboard");
            if (response != null && response.Any())
            {
                foreach (var userLeaderboardEntry in response!)
                {
                    var leaderboardItem = _mapper.Map<GrpcUserGetLeaderboardResponse>(userLeaderboardEntry);
                    if (!Leaderboard.Contains(leaderboardItem))
                        Leaderboard.Add(leaderboardItem);
                }
            }
            
        }

        public async Task<GenericAuthResponse<bool>> ShowBattleLogsWithRest(ShowBattleLogsRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync<ShowBattleLogsRequest>("api/battlelogs/show", request);
            var resultContent = await result.Content.ReadFromJsonAsync<GenericAuthResponse<bool>>();

            if (result.IsSuccessStatusCode)
                return new GenericAuthResponse<bool>() { Data = resultContent!.Data, Success = resultContent.Success };
            return new GenericAuthResponse<bool>() { Success = resultContent!.Success, Message = resultContent.Message };
        }

        public async Task<GenericAuthResponse<List<BattleLog>>> GetBattleLogsWithRest(GetBattleLogsRequest request)
        { 
            var response = await _httpClient.PostAsJsonAsync<GetBattleLogsRequest>("api/battlelogs/getLeaderboard", request);
            var responseContent = await response.Content.ReadFromJsonAsync<GenericAuthResponse<List<BattleLog>>>();

            if (response.IsSuccessStatusCode)
                return new GenericAuthResponse<List<BattleLog>>() { Data = responseContent!.Data, Success = true };
            return new GenericAuthResponse<List<BattleLog>>() { Success = false, Message = responseContent!.Message };
        }
    }
}
