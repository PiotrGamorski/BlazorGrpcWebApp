namespace BlazorGrpcWebApp.Shared.Models.UI_Models
{
    public class BattleResult
    {
        public List<string> Log { get; set; } = new List<string>();
        public int AttackerDamageSum { get; set; }
        public int OpponentDamageSum { get; set; }
        public bool IsVictory { get; set; }
        public int RoundsFought { get; set; }
    }
}
