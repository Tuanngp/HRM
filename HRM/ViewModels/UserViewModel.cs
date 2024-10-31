using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.Views;
using HRM.Views.User;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HRM.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private readonly Frame _navigationFrame;
    private readonly IAttendanceService _attendanceService;

    [ObservableProperty] private string? userName;
    [ObservableProperty] private string? userPosition;
    [ObservableProperty] private string? userAvatar;
    [ObservableProperty] private string? checkInStatus;
    [ObservableProperty] private int notificationCount;
    [ObservableProperty] private Visibility hasNotifications;

    public UserViewModel(Frame navigationFrame)
    {
        _navigationFrame = navigationFrame;
        _attendanceService = new AttendanceService();
        _ = LoadUserData();
    }

    public ICommand LogoutCommand { get; set; } = null!;

    [RelayCommand]
    private async Task CheckInOut()
    {
        try
        {
            if (CheckInStatus == "Chưa điểm danh")
            {
                CheckInStatus = "Đã điểm danh\n";
                await _attendanceService.CheckInAsync(UserSession.Instance.Employee!.Id);
                MessageBox.Show("Điểm danh thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var result = MessageBox.Show("Bạn có muốn điểm danh ra không?", "Điểm danh ra", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CheckInStatus = "Đã điểm danh ra\n";
                    await _attendanceService.CheckOutAsync(UserSession.Instance.Employee!.Id);
                    MessageBox.Show("Điểm danh ra thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            CheckInStatus += DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi trong quá trình điểm danh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    [RelayCommand]
    private void Navigate(string? destination)
    {
        switch (destination)
        {
            case "Home":
                _navigationFrame.Navigate(new UserDashboard());
                break;
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

    [RelayCommand]
    private void ShowNotifications()
    {
        // Implement notification logic here
    }

    private async Task LoadUserData()
    {
        try
        {
            var user = UserSession.Instance.User;
            NotificationCount = user!.NotificationReceiverUsers.Count;
            var emp = await UserSession.Instance.GetEmployee();
            CheckInStatus = await _attendanceService.HasCheckedInTodayAsync(UserSession.Instance.Employee!.Id)
                ? "Đã điểm danh"
                : "Chưa điểm danh";
            UserName = emp.FullName;
            UserPosition = "Lập trình viên";
            UserAvatar = emp.PhotoPath;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi trong quá trình tải dữ liệu người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}