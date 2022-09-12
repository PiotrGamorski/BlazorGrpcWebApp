namespace BlazorGrpcWebApp.Shared.Entities
{
    public class UserLastActivity
    {
        public int Id { get; set; }
        public DateTime ExecutionDate { get; set; } = DateTime.Now;
        public int? UserBananasTotal { get; set; }
        public int? UserBananasSpent { get; set; }
        public int? UserBananasGained { get; set; }
        public string? OpponentName { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int LastActivityId { get; set; }
        public virtual LastActivity LastActivity { get; set; }
    }
}
