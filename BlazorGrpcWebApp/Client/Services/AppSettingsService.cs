using BlazorGrpcWebApp.Client.Interfaces;

namespace BlazorGrpcWebApp.Client.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly IConfiguration _configuration;

        public AppSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetValueFromPagesSec(string pageName)
        {
            return _configuration[$"Settings:Pages:{pageName}:gRPC"];
        }

        public string GetValueFromSharedSec(string componentName)
        {
            return _configuration[$"Settings:Shared:{componentName}:gRPC"];
        }
    }
}
