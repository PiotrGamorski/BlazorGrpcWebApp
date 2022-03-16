namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcBattleService
    {
        Task DoGrpcStartBattle(int opponentId);
    }
}
