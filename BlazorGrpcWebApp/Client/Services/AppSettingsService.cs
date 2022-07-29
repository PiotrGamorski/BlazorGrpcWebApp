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

        public bool GetValueBySection(string sectionFullPath)
        {
            return bool.Parse(_configuration[sectionFullPath]);
        }
    }
}
