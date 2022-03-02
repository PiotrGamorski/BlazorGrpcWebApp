using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Login
    {
        public bool isAuthenticated = false;
        private UserLogin userLogin = new UserLogin();
        public int grpcLoginDeadline { get; set; } = 30000;

        private async Task HandleLoginRestApi()
        {
            if (!string.IsNullOrEmpty(userLogin.Email) && !string.IsNullOrEmpty(userLogin.Password))
            {
                try
                {
                    var result = await AuthService.Login(userLogin);
                    if (result.Success)
                    {
                        await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(result.Data);
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

        private async Task HandleLoginGrpc()
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
                        await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(result.Data);
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
