using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.Views;
using AdminViewModel = HRM.ViewModels.AdminViewModel;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM;

public partial class AdminWindow : Window
{
    private IAuthService _authService = new AuthService();
    public AdminWindow()
    {
        InitializeComponent();
        AdminFrame.Navigate(new AdminDashboard());
        DataContext = new AdminViewModel(AdminFrame);
        LogoutCommand = new RelayCommand(ExecuteLogout);
        ((AdminViewModel)DataContext).LogoutCommand = LogoutCommand;
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