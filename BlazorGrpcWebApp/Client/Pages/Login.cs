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
            if (userLogin.Email != null && userLogin.Email.Length > 0 && 
                userLogin.Password != null && userLogin.Password.Length > 0)
            {
                try
                {
                    var result = await AuthService.Login(userLogin);
                    if (result.Success)
                    {
                        await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(userLogin.Email);
                        await sessionStorage.SetItemAsync("username", userLogin.Email);
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
            if (userLogin.Email != null && userLogin.Email.Length > 0 &&
                userLogin.Password != null && userLogin.Password.Length > 0)
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
                        await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(userLogin.Email);
                        await sessionStorage.SetItemAsync("username", userLogin.Email);
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
