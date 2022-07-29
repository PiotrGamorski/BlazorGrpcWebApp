namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IAppSettingsService
    {
        bool GetValueBySection(string sectionFullPath);
    }
}
