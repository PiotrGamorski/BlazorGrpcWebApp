namespace BlazorGrpcWebApp.Client.Services
{
    public class LogoutService : ILogoutService
    {
        public event Action OnChange;
        public bool isAuthenticated { get; set; } = false;

        public async Task Authenticated()
        {
            isAuthenticated = true;
            await LogoutStatusChanged();
        }

        public async Task NotAuthenticated()
        {
            isAuthenticated = false;
            await LogoutStatusChanged();
        }

        public Task LogoutStatusChanged()
        {
            OnChange?.Invoke();
            return Task.CompletedTask;
        }
    }
}
