namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface ISignOutService
    {
        event Action OnChange;
        bool isAuthenticated { get; set; }

        Task Authenticated();
        Task NotAuthenticated();
        Task LogoutStatusChanged();
    }
}
