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
    private IEmployeeService employeeService;
    
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

    [ObservableProperty] private ObservableCollection<string> genderOptions = null!;
    [ObservableProperty] private ObservableCollection<string> maritalStatusOptions = null!;

    [ObservableProperty] private ObservableCollection<WorkHistoryItem> workHistory = null!;
    [ObservableProperty] private ObservableCollection<LoginHistoryItem> loginHistory = null!;

    public ICommand? GoBackCommand { get; private set; }
    public ICommand? ChangeProfileImageCommand { get; private set; }
    public ICommand? EditProfileCommand { get; private set; }
    public ICommand? SavePersonalInfoCommand { get; private set; }
    public ICommand? ChangePasswordCommand { get; private set; }

    public UserProfileViewModel()
    {
        InitializeCommands();
        authService = new AuthService();
        employeeService = new EmployeeService();
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

    private void ExecuteGoBack()
    {
    }

    private void ExecuteChangeProfileImage()
    {

    }

    private void ExecuteEditProfile()
    {

    }

    private void ExecuteSavePersonalInfo()
    {
        try
        {
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
        authService.ChangePasswordAsync(user!.Id, user.Password, (string)parameter!);
    }

    private async Task LoadUserData()
    {
        try
        {
            var employee = await employeeService.GetByIdAsync(UserSession.Instance.User!.Id);
            FullName = employee!.FullName;
            JobTitle = "Software Developer";
            Department = employee.Department?.Name;
            ProfileImage = employee.PhotoPath;
            DateOfBirth = new DateTime(employee.DateOfBirth.Year, employee.DateOfBirth.Month, employee.DateOfBirth.Day);
            IdNumber = employee.EmployeeCode;
            Address = employee.Address;
            Email = employee.Email;
            PhoneNumber = employee.Phone;
            SelectedGender = employee.Gender.ToString();
            SelectedMaritalStatus = "Độc thân";

            // Emergency contact
            EmergencyContactName = "Nguyễn Văn B";
            EmergencyContactRelation = "Cha";
            EmergencyContactPhone = "0987654321";
            EmergencyContactAddress = "456 XYZ Street";

            // Work information
            EmployeeId = "EMP001";
            ContractType = "Toàn thời gian";
            JoinDate = new DateTime(employee.HireDate.Year, employee.HireDate.Month, employee.HireDate.Day);
            Manager = "Trần Văn C";
            Status = employee.Status.ToString();
            ContractEndDate = new DateTime(2024, 12, 31);
            
            LoadOptions();
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
    
    private void LoadOptions()
    {
        GenderOptions = new ObservableCollection<string>
        {
            "Nam",
            "Nữ",
            "Khác"
        };
        MaritalStatusOptions = new ObservableCollection<string>
        {
            "Độc thân",
            "Đã kết hôn",
            "Ly thân",
            "Đã ly hôn"
        };
    }
}