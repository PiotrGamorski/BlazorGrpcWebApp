namespace BlazorGrpcWebApp.Client.Services
{
    public interface ILogoutService
    {
        event Action OnChange;
        bool isAuthenticated { get; set; }

        Task Authenticated();
        Task NotAuthenticated();
        Task LogoutStatusChanged();
    }
}
