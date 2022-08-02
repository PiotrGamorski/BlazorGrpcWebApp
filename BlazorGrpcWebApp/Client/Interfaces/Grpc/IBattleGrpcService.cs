namespace BlazorGrpcWebApp.Client.Interfaces.Grpc
{
    public interface IBattleGrpcService
    {
        List<string> LastBattleLogs { get; set; }
        Task<bool> DoGrpcStartBattle(int opponentId);
    }
}
