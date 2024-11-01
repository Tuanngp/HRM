using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.ViewModels;
using HRM.Views;
using HRM.Views.User;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM;

public partial class UserWindow : Window
{
    private IAuthService _authService = new AuthService();
    public UserWindow()
    {
        InitializeComponent();
        UserFrame.Navigate(new UserDashboard());
        DataContext = new UserViewModel(UserFrame);
        LogoutCommand = new RelayCommand(ExecuteLogout);
        ((UserViewModel)DataContext).LogoutCommand = LogoutCommand;
    }

    private void ExecuteLogout()
    {
        var result = MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == System.Windows.Forms.DialogResult.Yes)
        {
            var loginWindow = new LoginView();
            loginWindow.Show();
            _authService.LogoutAsync();
            this.Close();
        }
    }

    private ICommand? LogoutCommand { get; set; }
}