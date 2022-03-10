namespace BlazorGrpcWebApp.Shared.Entities
{
    public class UserUnit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public int HitPoints { get; set; }
    }
}
