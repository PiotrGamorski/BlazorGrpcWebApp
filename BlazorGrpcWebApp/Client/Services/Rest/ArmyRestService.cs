using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared.Dtos;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Rest
{
    public class ArmyRestService : IArmyRestService
    {
        private readonly HttpClient _httpClient;

        public ArmyRestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserUnitDto>> GetUserUnits()
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserUnitDto>>("api/userunit");
            return result!;
        }

        public async Task<HttpResponseMessage> HealUserUnit(int userUnitId)
        {
            var result = await _httpClient.PutAsJsonAsync("api/userunit/heal", userUnitId);
            return result;
        }

        public async Task<HttpResponseMessage> ReviveUserUnits()
        {
            var result = await _httpClient.GetAsync("api/userunit/reviveUserUnits");
            return result;
        }

        public async Task<HttpResponseMessage> DeleteUserUnit(int userUnitId)
        {
            var result = await _httpClient.DeleteAsync($"api/userunit/{userUnitId}");
            return result;
        }
    }
}
