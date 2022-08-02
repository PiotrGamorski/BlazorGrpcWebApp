using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services.Rest
{
    public class BattleRestService : IBattleRestService
    {
        private readonly HttpClient _httpClient;

        public BattleRestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GenericAuthResponse<BattleResult>> StartBattle(StartBattleRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync<StartBattleRequest>("api/battle", request);
            var responseContent = await response.Content.ReadFromJsonAsync<GenericAuthResponse<BattleResult>>();

            return responseContent!;
        }
    }
}
