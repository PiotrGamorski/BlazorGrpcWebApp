﻿using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Microsoft.JSInterop;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Login
    {
        private int grpcLoginDeadline { get; set; } = 50000;
        private UserLogin userLogin = new UserLogin();
        private IJSObjectReference? module;

        protected override Task OnInitializedAsync()
        {
            userLogin = new UserLogin();
            return Task.CompletedTask;
        }

        private async Task HandleLogin()
        {
            signingInStarted = true;
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/Login.razor.js");
            if (module != null) await module.InvokeAsync<string>("TurnOffSignInButton");

            if (bool.Parse(AppSettingsService.GetValueFromPagesSec("Login")))
            {
                await HandleLoginWithGrpc();
            }
            else
            {
                await HandleLoginWithRest();
            }
            signingInStarted = false;
            if (module != null) await module.InvokeAsync<string>("TurnOnSignInButton");
        }

        private async Task HandleLoginWithRest()
        {
            if (!string.IsNullOrEmpty(userLogin.Email) && !string.IsNullOrEmpty(userLogin.Password))
            {
                try
                {
                    var result = await AuthRestService.Login(userLogin);
                    if (result!.Success)
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
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        ToastService.ShowInfo(result.Message);
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
