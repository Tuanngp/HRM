// MainWindow.xaml.cs
using System.IO;
using System.Text.Json;
using System.Windows;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.Views;

namespace HRM;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly IAuthService _authService;

    public MainWindow()
    {
        InitializeComponent();
        _navigationService = new NavigationService();
        _authService = new AuthService();
        _ = OnStart();
    }

    private async Task OnStart()
    {
        if (File.Exists("UserSession.json"))
        {
            try
            {
                var sessionJson = await File.ReadAllTextAsync("UserSession.json");
                var sessionData = JsonSerializer.Deserialize<SessionData>(sessionJson);

                if (sessionData != null)
                {
                    User user = await _authService.LoginAsync(sessionData.Username, sessionData.Password);
                    await UserSession.Instance.SetUser(user);
                    UserSession.Instance.LastPageVisited = sessionData.LastPage;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        if (UserSession.Instance.User == null || UserSession.Instance.LastPageVisited == null)
        {
            LoginView loginView = new LoginView();
            loginView.Show();
            this.Close();
            return;
        }

        var result = UserSession.Instance.User;
        if (result != null)
        {
            switch (result.Role)
            {
                case "Admin":
                    var adminWindow = new AdminWindow();
                    adminWindow.AdminFrame.Navigated += _navigationService.OnNavigated;
                    adminWindow.Show();
                    this.Close();
                    _navigationService.NavigateTo(UserSession.Instance.LastPageVisited);
                    break;
                case "User":
                    var userWindow = new UserWindow();
                    userWindow.UserFrame.Navigated += _navigationService.OnNavigated;
                    userWindow.Show();
                    this.Close();
                    _navigationService.NavigateTo(UserSession.Instance.LastPageVisited);
                    break;
            }
        }
    }
}