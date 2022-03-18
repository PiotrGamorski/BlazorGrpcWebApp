using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcBattleService
    {
        Task<bool> DoGrpcStartBattle(int opponentId);
    }
}
