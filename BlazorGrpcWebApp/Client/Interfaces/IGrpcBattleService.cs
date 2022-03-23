using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcBattleService
    {
        List<string> LastBattleLogs { get; set; }
        Task<bool> DoGrpcStartBattle(int opponentId);
    }
}
