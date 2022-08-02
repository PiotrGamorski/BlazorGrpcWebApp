namespace BlazorGrpcWebApp.Shared.Models.Controllers_Models
{
    public class StartBattleRequest
    {
        public int AuthUserId { get; set; }
        public int OpponentId { get; set; }
    }
}
