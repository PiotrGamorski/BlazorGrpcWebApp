using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Login
    {
        public bool isAuthenticated = false;
        private UserLogin userLogin = new UserLogin();
        public int grpcLoginDeadline { get; set; } = 50000;

        private async Task HandleLogin()
        {
            if (bool.Parse(AppSettingsService.GetValueFromPagesSec("Login")))
                await HandleLoginWithGrpc();
            else await HandleLoginWithRest();
        }

        private async Task HandleLoginWithRest()
        {
            if (!string.IsNullOrEmpty(userLogin.Email) && !string.IsNullOrEmpty(userLogin.Password))
            {
                try
                {
                    var result = await AuthService.Login(userLogin);
                    if (result.Success)
                    {
                        try
                        {
                            await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(result.Data!);
                        }
                        catch (Exception e)
                        {
                            ToastService.ShowError(e.Message);
                        }

                        await sessionStorage.SetItemAsync("authToken", result.Data);
                        await LogoutService.Authenticated();
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        ToastService.ShowError(result.Message);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private async Task HandleLoginWithGrpc()
        {
            if (!string.IsNullOrEmpty(userLogin.Email) && !string.IsNullOrEmpty(userLogin.Password))
            {
                try
                {
                    var result = await GrpcUserService.DoGrpcUserLogin(new LoginGrpcUserRequest()
                    {
                        GrpcUser = new GrpcUser() { Email = userLogin.Email },
                        Password = userLogin.Password,
                    }, grpcLoginDeadline);
                    if (result.Success)
                    {
                        try
                        {
                            await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(result.Data);
                        }
                        catch (Exception e)
                        {
                            ToastService.ShowError(e.Message);
                        }
                        await sessionStorage.SetItemAsync("authToken", result.Data);
                        await LogoutService.Authenticated();
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        ToastService.ShowError(result.Message);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
