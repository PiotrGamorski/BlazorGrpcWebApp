using BlazorGrpcWebApp.Client.Interfaces.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Client.Services.Shared
{
    public class TopMenuService : ITopMenuService
    {
        public string AuthUserName { get; set; } = string.Empty;
        public string AuthUserInitials { get; set; } = string.Empty;

        public void SetAuthUserNameAndInitials(AuthenticationState authState)
        {
            AuthUserName = authState.User.FindFirst(c => c.Type == ClaimTypes.Name)!.Value;
            AuthUserInitials = AuthUserName.Substring(0, 1);
        }
    }
}
