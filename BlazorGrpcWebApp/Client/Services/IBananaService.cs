namespace BlazorGrpcWebApp.Client.Services
{
    public interface IBananaService
    {
        event Action OnChange;
        int Bananas { get; set; }
        Task EatBananas(int amount);
        Task AddBananas (int amount);
        Task GetBananas();
    }
}
