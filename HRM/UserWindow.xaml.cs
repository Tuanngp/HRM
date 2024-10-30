using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.ViewModels;
using HRM.Views;
using HRM.Views.User;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM;

public partial class UserWindow : Window
{
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