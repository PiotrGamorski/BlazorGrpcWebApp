using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Server;
using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Server.Services;
using BlazorGrpcWebApp.Shared.gRPC_Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                           options => options.MigrationsAssembly("BlazorGrpcWebApp.Server")));
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

var app = builder.Build();

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
	endpoints.MapRazorPages();
	endpoints.MapControllers();
	endpoints.MapFallbackToFile("index.html");
});


app.MapRazorPages();

app.Run();
