using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Services
{
    public interface IUnitService
    {
        IList<Unit> Units { get; set; }
        IList<UserUnit> MyUnits { get; set;}
        Task AddUnit(int unitId);
        Task LoadUnitAsync();
    }
}
