using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Models.Enum;
using HRM.Service;
using HRM.Service.ServiceImpl;
using Microsoft.Win32;

namespace HRM.ViewModels;

public partial class EmployeeDetailViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IUserService _userService = new UserService();

    [ObservableProperty] private Employee? employee;
    [ObservableProperty] private bool isEditable;
    [ObservableProperty] private bool isNewEmployee;
    [ObservableProperty] private string title = null!;
    [ObservableProperty] private int employeeId;
    [ObservableProperty] private string? avatarPath;
    [ObservableProperty] private Department? selectedDepartment;
    

    [ObservableProperty] private ObservableCollection<Department?> departments = null!;
    [ObservableProperty] private ObservableCollection<EmployeeStatus> employeeStatuses = null!;
    [ObservableProperty] private ObservableCollection<string> genders = null!;

    public event EventHandler? EmployeeSaved;
    public EmployeeDetailViewModel(int empId, bool isEdit)
    {
        _employeeService = new EmployeeService();
        _departmentService = new DepartmentService();

        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
        CancelCommand = new RelayCommand(ExecuteCancel);
        ChangePhotoCommand = new RelayCommand(ExecuteChangePhoto);
        EmployeeId = empId;
        IsEditable = isEdit;
        Initialize();
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ChangePhotoCommand { get; }

    public void Initialize()
    {
        LoadCollections();
        IsNewEmployee = EmployeeId <= 0;
        if (IsNewEmployee)
        {
            AvatarPath = "/Assets/avatar/default.jpg";
            Title = "Thêm thông tin nhân viên mới";
            Employee = new Employee
            {
                DateOfBirth = new DateOnly(1990, 1, 1),
                HireDate = DateOnly.FromDateTime(DateTime.Now),
                Status = EmployeeStatus.Active,
                CreatedDate = DateTime.Now
            };
        }
        else
        {
            LoadEmployee();
            AvatarPath = Employee?.PhotoPath;
            Title = "Cập nhật thông tin nhân viên";
        }
    }

    private async void LoadEmployee()
    {
        try
        {
            Employee = await _employeeService.GetByIdAsync(EmployeeId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi trong quá trình tải dữ liệu nhân viên: {ex.Message}", "Lỗi");
        }
    }

    private async void LoadCollections()
    {
        try
        {
            var departmentsLoading = await _departmentService.GetAllDepartments();
            Departments = new ObservableCollection<Department?>(departmentsLoading);
            EmployeeStatuses = new ObservableCollection<EmployeeStatus>
            {
                EmployeeStatus.Active,
                EmployeeStatus.Inactive,
                EmployeeStatus.OnLeave,
                EmployeeStatus.Terminated
            };
            Genders = new ObservableCollection<string>
            {
                "Nam",
                "Nữ",
                "Khác"
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi trong quá trình tải dữ liệu phòng ban: {ex.Message}", "Lỗi");
        }
    }

    private Gender GetGender(string str)
    {
        if (str == "Nam")
        {
            return Gender.Male;
        }

        if (str == "Nữ")
        {
            return Gender.Female;
        }
        return Gender.Other;
    }

    private async void ExecuteSave()
    {
        try
        {
            if (!ValidateEmployee())
            {
                return;
            }

            if (IsNewEmployee)
            {
                var user = await _userService.AddAsync(new User()
                {
                    Username = Employee!.Email,
                    Password = "123456",
                    CreatedDate = DateTime.Now,
                    Role = "User"
                });
                Employee!.UserId = user!.Id;
                await _employeeService.CreateEmployeeAsync(Employee);
            }
            else
            {
                await _employeeService.UpdateEmployeeAsync(EmployeeId, Employee);
            }

            MessageBox.Show(
                IsNewEmployee ? "Thêm thông tin nhân viên thành công!" : "Cập nhật thông tin nhân viên thành công!",
                "Thành công");
            EmployeeSaved?.Invoke(this, EventArgs.Empty);
            CloseWindow();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lưu thông tin nhân viên thất bại: {ex.Message}", "Lỗi");
        }
    }

    private bool CanExecuteSave()
    {
        return IsEditable;
    }

    private void ExecuteCancel()
    {
        CloseWindow();
    }

    private async void ExecuteChangePhoto()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Tệp hình ảnh (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Tất cả các tệp (*.*)|*.*",
            Title = "Chọn Ảnh Đại Diện Nhân Viên"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string newPhotoPath = SavePhotoToStorage(openFileDialog.FileName);
                Employee!.PhotoPath = newPhotoPath;
                if (IsNewEmployee)
                {
                    Employee.PhotoPath = newPhotoPath;
                    AvatarPath = newPhotoPath;
                    MessageBox.Show("Thêm ảnh đại diện thành công!", "Thành công", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                await _employeeService.UploadAvatarAsync(EmployeeId, newPhotoPath);
                AvatarPath = newPhotoPath;
                MessageBox.Show("Thay ảnh đại diện thành công!", "Thành công", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thay ảnh đại diện thất bại: " + ex, "Lỗi");
            }
        }
    }

    private string SavePhotoToStorage(string sourceFilePath)
    {
        string photoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "avatar");

        if (!Directory.Exists(photoDirectory))
        {
            Directory.CreateDirectory(photoDirectory);
        }

        string fileName = Path.GetFileName(sourceFilePath);
        string destinationFilePath = Path.Combine(photoDirectory, fileName);

        int count = 1;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);

        while (File.Exists(destinationFilePath))
        {
            string tempFileName = $"{fileNameWithoutExtension}({count++}){extension}";
            destinationFilePath = Path.Combine(photoDirectory, tempFileName);
        }

        File.Copy(sourceFilePath, destinationFilePath, true);

        return destinationFilePath;
    }

    private bool ValidateEmployee()
    {
        if (string.IsNullOrWhiteSpace(Employee!.FirstName) || string.IsNullOrWhiteSpace(Employee.LastName))
        {
            MessageBox.Show("Vui lòng nhập họ và tên.", "Lỗi xác thực");
            return false;
        }

        if (Employee.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
        {
            MessageBox.Show("Ngày sinh không thể vượt quá ngày hiện tại.", "Lỗi xác thực");
            return false;
        }

        if (Employee.HireDate > DateOnly.FromDateTime(DateTime.Now))
        {
            MessageBox.Show("Ngày bắt đầu không được vượt quá ngày hiện tại.", "Lỗi xác thực");
            return false;
        }

        if (Employee.BasicSalary < 0)
        {
            MessageBox.Show("Mức lương phải là số dương.", "Lỗi xác thực");
            return false;
        }

        if (SelectedDepartment == null)
        {
            MessageBox.Show("Vui lòng chọn phòng ban.", "Lỗi xác thực");
            return false;
        }

        return true;
    }

    private void CloseWindow()
    {
        if (Application.Current.Windows.Count > 0)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}