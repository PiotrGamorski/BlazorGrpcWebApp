using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Dtos
{
    public class UserLastActivitiesRequestDto
    {
        public int UserId { get; set; }
        public Page Page { get; set; }
        public int ActivitiesNumber { get; set; }
    }
}
