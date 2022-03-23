namespace BlazorGrpcWebApp.Shared.Entities
{
    public class BattleLog
    {
        public int Id { get; set; }
        public Battle Battle { get; set; }
        public int BattleId { get; set; }
        public User Attacker { get; set; }
        public int AttackerId { get; set; }
        public User Opponent { get; set; }
        public int OpponentId { get; set; }
        public string Log { get; set; }

    }
}
