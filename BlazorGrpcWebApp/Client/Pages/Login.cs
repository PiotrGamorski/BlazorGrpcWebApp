using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Login
    {
        public bool isAuthenticated = false;
        private UserLogin userLogin = new UserLogin();

        private async Task HandleLogin()
        {
            if (userLogin.Username != null && userLogin.Username.Length > 0 && 
                userLogin.Password != null && userLogin.Password.Length > 0)
            {
                try
                {
                    await ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(userLogin.Username);
                    await sessionStorage.SetItemAsync("username", userLogin.Username);
                    await LogoutService.Authenticated();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
