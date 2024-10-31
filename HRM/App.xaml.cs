using System.Windows;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;

namespace HRM;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App()
    {
        _authService = new AuthService();
        this.Exit += OnExit;
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        var user = UserSession.Instance.User;
        if (user != null)
        {
            await _authService.SaveUserSessionAsync(user, UserSession.Instance.LastPageVisited!);
        }
    }
}