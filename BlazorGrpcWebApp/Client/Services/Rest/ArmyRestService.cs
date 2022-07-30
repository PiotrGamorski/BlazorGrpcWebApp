using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public async Task<List<UserUnitDto>> GetArmy()
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserUnitDto>>("api/userunit");
            return result!;
        }

        [Authorize]
        public async Task<HttpResponseMessage> ReviveArmy()
        {
            var result = await _httpClient.PostAsJsonAsync<string>("api/battle/reviveArmy", null!);
            return result;
        }
    }
}
