using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Dtos
{
    public class UserLastActivityDto
    {
        public DateTime ExecutionDate { get; set; }
        public Activity LastActivity { get; set; }
        public int? UserBananasTotal { get; set; }
        public int? UserBananasSpent {get; set;}
        public int? UserBananasGained { get; set; }
        public string? OpponentName { get; set; }
    }
}
