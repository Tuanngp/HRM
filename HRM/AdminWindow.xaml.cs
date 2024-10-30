using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Views;
using AdminViewModel = HRM.ViewModels.AdminViewModel;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM;

public partial class AdminWindow : Window
{
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
        var result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == System.Windows.Forms.DialogResult.Yes)
        {
            var loginWindow = new LoginView();
            loginWindow.Show();
            UserSession.Instance.Clear();
            this.Close();
        }
    }

    private ICommand? LogoutCommand { get; set; }
}