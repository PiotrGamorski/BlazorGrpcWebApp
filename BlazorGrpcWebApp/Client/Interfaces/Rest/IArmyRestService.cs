using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IArmyRestService
    {
        Task<List<UserUnitDto>> GetArmy();
        Task<HttpResponseMessage> ReviveArmy();
    }
}
