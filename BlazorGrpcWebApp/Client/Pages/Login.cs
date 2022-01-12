using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Login
    {
        public bool isAuthenticated = false;
        private UserLogin userLogin = new UserLogin();

        private async Task HandleLogin()
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
    }
}
