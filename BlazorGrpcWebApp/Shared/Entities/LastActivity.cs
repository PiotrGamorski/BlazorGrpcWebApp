using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Entities
{
    public class LastActivity
    {
        public int Id { get; set; }
        public Activity ActivityType { get; set; }
    }
}
