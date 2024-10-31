using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.Views;

namespace HRM.ViewModels;

public partial class LoginViewModel(LoginView loginView) : ObservableObject
{
    private readonly IAuthService _authService = new AuthService();

    [ObservableProperty] private string username = null!;

    [ObservableProperty] private string password = null!;

    [ObservableProperty] private string errorMessage = null!;

    [RelayCommand]
    private async Task Login()
    {
        if (!ValidationInput())
        {
            return;
        }
        try
        {
            ErrorMessage = string.Empty;
            var result = await _authService.LoginAsync(Username, Password);
            if (result != null)
            {
                await UserSession.Instance.SetUser(result);
                switch (result.Role)
                {
                    case "Admin":
                        var adminWindow = new AdminWindow();
                        adminWindow.Show();
                        break;
                    case "User":
                        var userWindow = new UserWindow();
                        userWindow.Show();
                        break;
                    default:
                        ErrorMessage = "Vai trò không hợp lệ";
                        break;
                }

                loginView.Close();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Đăng nhập thất bại: {ex.Message}";
        }
    }

    private bool ValidationInput()
    {

        if (string.IsNullOrEmpty(Username))
        {
            ErrorMessage = "Vui lòng nhập tên tài khoản! ";
            return false;
        }

        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Vui lòng nhập mật khẩu! ";
            return false;
        }

        return true;
    }
}