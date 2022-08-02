using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IBattleRestService
    {
        Task<GenericAuthResponse<BattleResult>> StartBattle(StartBattleRequest request);
    }
}
