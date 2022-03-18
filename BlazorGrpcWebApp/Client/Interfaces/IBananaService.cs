namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IBananaService
    {
        event Action OnChange;
        int Bananas { get; set; }
        Task BananasChanged();
        Task EatBananas(int amount);
        Task AddBananas (int amount);
        Task GrpcAddBananas(int amount);
        Task GetBananas();
        Task GrpcGetBananas();
    }
}
