using Blazored.Toast.Services;
using BlazorGrpcWebApp.Shared;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class UnitService : IUnitService
    {
        private readonly IToastService _toastService;
        private readonly HttpClient _httpClient;

        public UnitService(IToastService toastService, HttpClient httpClient)
        {
            _toastService = toastService;
            _httpClient = httpClient;
        }

        public IList<Unit> Units { get; set; } = new List<Unit>();
    
        public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

        public Task AddUnit(int unitId)
        {
            var unit = Units.First(u => u.Id == unitId);
            if (unit != null)
            {
                MyUnits.Add(new UserUnit { UnitId = unit.Id, HitPoints = unit.HitPoints });
                _toastService.ShowSuccess($"Your {unit.Title} has been built!", "Unit built!");
            }
            else
                throw new InvalidOperationException("Cannot add Unit which does not exist!");

            return Task.CompletedTask;
        }

        public async Task LoadUnitsAsync()
        {
            if (Units.Count == 0)
            {
                Units = await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit");
            }
        }
    }
}
