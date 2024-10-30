using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Views;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private readonly Frame _navigationFrame;
    [ObservableProperty] private string? userName;
    [ObservableProperty] private string? userPosition;
    [ObservableProperty] private string? userAvatar;
    [ObservableProperty] private string? checkInStatus;
    [ObservableProperty] private int notificationCount;
    [ObservableProperty] private Visibility hasNotifications;

    public UserViewModel(Frame navigationFrame)
    {
        _navigationFrame = navigationFrame;
        InitializeCommands();
        LoadUserData();
    }
    
    public ICommand? CheckInOutCommand { get; private set; }
    public ICommand? NavigateCommand { get; private set; }
    public ICommand? ShowNotificationsCommand { get; private set; }
    public ICommand? LogoutCommand { get; set; }

    private void InitializeCommands()
    {
        CheckInOutCommand = new RelayCommand(ExecuteCheckInOut);
        NavigateCommand = new RelayCommand<string>(ExecuteNavigate);
        ShowNotificationsCommand = new RelayCommand(ExecuteShowNotifications);
    }

    private void ExecuteCheckInOut()
    {
        try
        {
            CheckInStatus = DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during check in/out: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExecuteNavigate(string? destination)
    {
        switch (destination)
        {
            case "Profile":
                _navigationFrame.Navigate(new UserProfileView());
                break;
            case "Schedule":
                _navigationFrame.Navigate(new EmployeeCalendarView());
                break;
            case "Leave":
                _navigationFrame.Navigate(new EmployeeLeaveRequestFormView());
                break;
            case "Payroll":
                _navigationFrame.Navigate(new EmployeeSalaryInformationView());
                break;
        }
    }

    private void ExecuteShowNotifications()
    {

    }

    private void LoadUserData()
    {
        try
        {
            UserName = "John Doe";
            UserPosition = "Software Developer";
            UserAvatar = "/Assets/default-avatar.png";
            NotificationCount = 3;
            CheckInStatus = "Not checked in";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}