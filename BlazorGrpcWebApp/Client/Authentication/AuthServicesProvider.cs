using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Client.Interfaces.Shared;

namespace BlazorGrpcWebApp.Client.Authentication
{
    public class AuthServicesProvider
    {
        public readonly IBananaService _bananaService;
        public readonly ITopMenuService _topMenuService;

        public AuthServicesProvider(IBananaService bananaService, ITopMenuService topMenuService)
        {
            _bananaService = bananaService;
            _topMenuService = topMenuService;
        }
    }
}
