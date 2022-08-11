using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IUserRolesService
    {
        bool HasAdminRole { get; set; }
        bool HasUserRole { get; set; }

        void SetUserRoles(AuthenticationState authState);
    }
}
