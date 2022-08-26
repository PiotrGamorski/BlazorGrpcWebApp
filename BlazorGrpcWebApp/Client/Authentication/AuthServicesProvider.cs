using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Client.Interfaces.Shared;

namespace BlazorGrpcWebApp.Client.Authentication
{
    public interface IAuthServicesProvider 
    {
        IBananaService BananaService { get; }
        ITopMenuService TopMenuService { get; }
        IUserRolesService UserRolesService { get; }
    }

    public class AuthServicesProvider : IAuthServicesProvider
    {
        public IBananaService BananaService { get; }
        public ITopMenuService TopMenuService { get; }
        public IUserRolesService UserRolesService { get; }

        public AuthServicesProvider(IBananaService bananaService, ITopMenuService topMenuService, IUserRolesService userRolesService)
        {
            BananaService = bananaService;
            TopMenuService = topMenuService;
            UserRolesService = userRolesService;
        }
    }
}
