using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorGrpcWebApp.Client.Interfaces.Shared
{
    public interface ITopMenuService
    {
        string AuthUserName { get; set; }
        string AuthUserInitials { get; set; }

        void SetAuthUserNameAndInitials(AuthenticationState authState);
    }
}
