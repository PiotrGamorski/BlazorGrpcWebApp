using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Server;
using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Server.Services;
using BlazorGrpcWebApp.Shared.gRPC_Services;
using BlazorGrpcWebApp.Server.EntitySeeder;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

if (bool.Parse(configuration.GetSection("AppSettings:DbMigrationsMode").Value))
    builder.Services.AddDbContext<DataContextForMigrations>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        options => options.MigrationsAssembly("BlazorGrpcWebApp.Server")));
else
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        options => options.MigrationsAssembly("BlazorGrpcWebApp.Server")));

builder.Services.AddTransient<ISeeder, EntitySeeder>();
builder.Services.AddGrpc();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddScoped<IUtilityService, UtilityService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IBattleService, BattleService>();

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata" && 
    bool.Parse(configuration.GetSection("AppSettings:DbMigrationsMode").Value))
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeeder>();
        service!.Seed();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseGrpcWeb();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	// map to and register the gRPC service
	endpoints.MapGrpcService<WeatherService>().EnableGrpcWeb();
	endpoints.MapGrpcService<UnitServiceGrpcImpl>().EnableGrpcWeb();
    endpoints.MapGrpcService<UserServiceGrpcImpl>().EnableGrpcWeb();
    endpoints.MapGrpcService<UserUnitServiceGrpcImpl>().EnableGrpcWeb();
    endpoints.MapGrpcService<BattleServiceGrpcImpl>().EnableGrpcWeb();
    endpoints.MapGrpcService<ArmyServiceGrpcImpl>().EnableGrpcWeb();
    endpoints.MapGrpcService<BattleLogServiceGrpcImpl>().EnableGrpcWeb();
	endpoints.MapRazorPages();
	endpoints.MapControllers();
	endpoints.MapFallbackToFile("index.html");
});


app.MapRazorPages();

app.Run();
