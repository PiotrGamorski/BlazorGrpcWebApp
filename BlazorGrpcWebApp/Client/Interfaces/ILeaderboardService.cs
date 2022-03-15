using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface ILeaderboardService
    {
        IList<UserStatistic>? Leaderboard { get; set; }
        IList<GrpcUserGetLeaderboardResponse> GrpcLeaderboardResponses { get; set; }
        Task GetLeaderboardRestApi();
        Task DoGrpcGetLeaderboard();
    }
}
