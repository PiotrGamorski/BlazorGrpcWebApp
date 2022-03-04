namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface ILogoutNavMenuService
    {
        event Action OnChange;
        bool isAuthenticated { get; set; }

        Task Authenticated();
        Task NotAuthenticated();
        Task LogoutStatusChanged();
    }
}
