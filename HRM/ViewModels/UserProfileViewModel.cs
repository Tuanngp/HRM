using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;

namespace HRM.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    private IAuthService authService;
    
    [ObservableProperty] private string? fullName;
    [ObservableProperty] private string? jobTitle;
    [ObservableProperty] private string? department;
    [ObservableProperty] private string? profileImage;
    [ObservableProperty] private DateTime dateOfBirth;
    [ObservableProperty] private string? idNumber;
    [ObservableProperty] private string? address;
    [ObservableProperty] private string? email;
    [ObservableProperty] private string? phoneNumber;
    [ObservableProperty] private string? selectedGender;
    [ObservableProperty] private string? selectedMaritalStatus;
    [ObservableProperty] private string? emergencyContactName;
    [ObservableProperty] private string? emergencyContactRelation;
    [ObservableProperty] private string? emergencyContactPhone;
    [ObservableProperty] private string? emergencyContactAddress;
    [ObservableProperty] private string? employeeId;
    [ObservableProperty] private string? contractType;
    [ObservableProperty] private DateTime joinDate;
    [ObservableProperty] private string? manager;
    [ObservableProperty] private string? status;
    [ObservableProperty] private DateTime contractEndDate;
    [ObservableProperty] private bool isTwoFactorEnabled;

    public ObservableCollection<string> GenderOptions { get; } = new ObservableCollection<string>
    {
        "Nam",
        "Nữ",
        "Khác"
    };

    public ObservableCollection<string> MaritalStatusOptions { get; } = new ObservableCollection<string>
    {
        "Độc thân",
        "Đã kết hôn",
        "Ly hôn",
        "Góa"
    };

    public ObservableCollection<WorkHistoryItem> WorkHistory { get; } = new ObservableCollection<WorkHistoryItem>();
    public ObservableCollection<LoginHistoryItem> LoginHistory { get; } = new ObservableCollection<LoginHistoryItem>();

    public ICommand? GoBackCommand { get; private set; }
    public ICommand? ChangeProfileImageCommand { get; private set; }
    public ICommand? EditProfileCommand { get; private set; }
    public ICommand? SavePersonalInfoCommand { get; private set; }
    public ICommand? ChangePasswordCommand { get; private set; }

    public UserProfileViewModel()
    {
        InitializeCommands();
        authService = new AuthService();
        LoadUserData();
    }

    private void InitializeCommands()
    {
        GoBackCommand = new RelayCommand(ExecuteGoBack);
        ChangeProfileImageCommand = new RelayCommand(ExecuteChangeProfileImage);
        EditProfileCommand = new RelayCommand(ExecuteEditProfile);
        SavePersonalInfoCommand = new RelayCommand(ExecuteSavePersonalInfo);
        ChangePasswordCommand = new RelayCommand<object>(ExecuteChangePassword);
    }

    #region Command Implementations

    private void ExecuteGoBack()
    {
        // Implement navigation back logic
    }

    private void ExecuteChangeProfileImage()
    {
        // Implement profile image change logic
    }

    private void ExecuteEditProfile()
    {
        // Implement profile editing logic
    }

    private void ExecuteSavePersonalInfo()
    {
        try
        {
            // Implement save logic here
            MessageBox.Show("Thông tin đã được lưu thành công!", "Thành công",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExecuteChangePassword(object? parameter)
    {
        var user = UserSession.Instance.User;
        authService.ChangePasswordAsync(user.Id, user.Password, (string)parameter);
    }

    #endregion

    private void LoadUserData()
    {
        // Load user data from service
        try
        {
            // Simulate loading data
            FullName = "Nguyễn Văn A";
            JobTitle = "Software Developer";
            Department = "IT Department";
            ProfileImage = "/Assets/profile.jpg";
            DateOfBirth = new DateTime(1990, 1, 1);
            IdNumber = "123456789";
            Address = "123 ABC Street";
            Email = "nguyenvana@example.com";
            PhoneNumber = "0123456789";
            SelectedGender = "Nam";
            SelectedMaritalStatus = "Độc thân";

            // Emergency contact
            EmergencyContactName = "Nguyễn Văn B";
            EmergencyContactRelation = "Cha";
            EmergencyContactPhone = "0987654321";
            EmergencyContactAddress = "456 XYZ Street";

            // Work information
            EmployeeId = "EMP001";
            ContractType = "Toàn thời gian";
            JoinDate = new DateTime(2020, 1, 1);
            Manager = "Trần Văn C";
            Status = "Đang làm việc";
            ContractEndDate = new DateTime(2024, 12, 31);

            LoadWorkHistory();
            LoadLoginHistory();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadWorkHistory()
    {
        WorkHistory.Clear();
        // Add sample work history
        WorkHistory.Add(new WorkHistoryItem
        {
            Period = "01/2020 - Hiện tại",
            Department = "IT Department",
            Position = "Software Developer",
            Manager = "Trần Văn C"
        });
    }

    private void LoadLoginHistory()
    {
        LoginHistory.Clear();
        // Add sample login history
        LoginHistory.Add(new LoginHistoryItem
        {
            LoginTime = DateTime.Now,
            Device = "Windows 10 - Chrome",
            IpAddress = "192.168.1.1",
            Status = "Thành công"
        });
    }
}

public class WorkHistoryItem
{
    public string Period { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }
    public string Manager { get; set; }
}

public class LoginHistoryItem
{
    public DateTime LoginTime { get; set; }
    public string Device { get; set; }
    public string IpAddress { get; set; }
    public string Status { get; set; }
}