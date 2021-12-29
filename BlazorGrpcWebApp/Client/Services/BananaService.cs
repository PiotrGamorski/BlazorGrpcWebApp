namespace BlazorGrpcWebApp.Client.Services
{
    public class BananaService : IBananaService
    {
        // one needs to introduce event Action as StateHasChanged is only avaiable in razor files.
        public event Action OnChange;
        public int Bananas { get; set; } = 1000;

        public async Task EatBananas(int amount)
        {
            Bananas -= amount;
            await BananasChanged();
        }

        public async Task AddBananas(int amount)
        {
            Bananas += amount;
            await BananasChanged();
        }

        Task BananasChanged()
        { 
            OnChange?.Invoke();
            return Task.CompletedTask;
        }
    }
}
