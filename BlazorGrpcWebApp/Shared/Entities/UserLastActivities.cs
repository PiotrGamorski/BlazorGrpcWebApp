namespace BlazorGrpcWebApp.Shared.Entities
{
    public class UserLastActivitie
    {
        public int Id { get; set; }
        public DateTime ExecutionDate { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int LastActivityId { get; set; }
        public virtual LastActivity LastActivity { get; set; }
    }
}
