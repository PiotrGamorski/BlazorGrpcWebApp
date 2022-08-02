using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface ILeaderboardRestService
    {
        IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; }

        Task GetLeaderboardWithRest();
        Task<GenericAuthResponse<bool>> ShowBattleLogsWithRest(ShowBattleLogsRequest request);
        Task<GenericAuthResponse<List<BattleLog>>> GetBattleLogsWithRest(GetBattleLogsRequest request);
    }
}
