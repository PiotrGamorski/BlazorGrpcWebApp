using BlazorGrpcWebApp.Client.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorGrpcWebApp.Client.Services
{
    public class UserRolesService : IUserRolesService
    {
        public bool HasAdminRole { get; set; } = false;
        public bool HasUserRole { get; set; } = false;

        public UserRolesService() {}

        public void SetUserRoles(AuthenticationState authState)
        {
            HasAdminRole = !string.IsNullOrEmpty(authState.User.FindFirst(c => c.Type == "AdminRole")!.Value);
            HasUserRole = !string.IsNullOrEmpty(authState.User.FindFirst(c => c.Type == "UserRole")!.Value);
        }
    }
}
