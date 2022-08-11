using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Client.Interfaces.Shared;

namespace BlazorGrpcWebApp.Client.Authentication
{
    public class AuthServicesProvider
    {
        public readonly IBananaService _bananaService;
        public readonly ITopMenuService _topMenuService;
        public readonly IUserRolesService _userRolesService;

        public AuthServicesProvider(IBananaService bananaService, ITopMenuService topMenuService, IUserRolesService userRolesService)
        {
            _bananaService = bananaService;
            _topMenuService = topMenuService;
            _userRolesService = userRolesService;
        }
    }
}
