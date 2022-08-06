using BlazorGrpcWebApp.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;
using MudBlazor.Services;
using BlazorGrpcWebApp.Client.Services;
using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Client.Interfaces.Rest;
using BlazorGrpcWebApp.Client.Services.Rest;
using BlazorGrpcWebApp.Client.Interfaces.Grpc;
using BlazorGrpcWebApp.Client.Services.Grpc;
using BlazorGrpcWebApp.Client.MappingProfile;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton(services =>
{
	var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
	var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
	var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
	return new WeatherForecasts.WeatherForecastsClient(channel);
});

var httpClient = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddScoped(sp => httpClient);
using var response = await httpClient.GetAsync("appsettings.json");
using var stream = await response.Content.ReadAsStreamAsync();

builder.Configuration.AddJsonStream(stream);
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthRestService, AuthRestService>();
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
builder.Services.AddScoped<IArmyGrpcService, ArmyGrpcService>();
builder.Services.AddScoped<IArmyRestService, ArmyRestService>();
builder.Services.AddScoped<IBananaService, BananaService>();
builder.Services.AddScoped<IBananaRestService, BananaRestService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUserGrpcService, UserGrpcService>();
builder.Services.AddScoped<IGrpcUserUnitService, GrpcUserUnitService>();
builder.Services.AddScoped<ILeaderboardGrpcService, LeaderboardGrpcService>();
builder.Services.AddScoped<ILeaderboardRestService, LeaderboardRestService>();
builder.Services.AddScoped<IBattleGrpcService, BattleGrpcService>();
builder.Services.AddScoped<IBattleRestService, BattleRestService>();
builder.Services.AddAutoMapper(typeof(ClientAppMappingProfile));
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddMudServices();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
