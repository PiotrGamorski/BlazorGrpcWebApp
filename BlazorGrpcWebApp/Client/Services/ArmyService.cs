using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared.Dtos;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class ArmyService : IArmyService
    {
        private readonly HttpClient _httpClient;

        public ArmyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserUnitResponse>> RestApiGetUserUnits()
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserUnitResponse>>("api/userunit");
            return result!;
        }
    }
}
