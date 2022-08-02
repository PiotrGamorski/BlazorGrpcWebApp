namespace BlazorGrpcWebApp.Shared.Models.Controllers_Models
{
    public  class GetBattleLogsRequest
    {
        public int AuthUserId { get; set; }
        public int OpponentId { get; set; }
    }
}
