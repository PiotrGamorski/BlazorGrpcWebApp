namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IAppSettingsService
    {
        string GetValueFromSharedSec(string componentName);
        string GetValueFromPagesSec(string pageName);
    }
}
