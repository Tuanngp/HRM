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

    [ObservableProperty] private Employee? employee;
    [ObservableProperty] private bool isEditable;
    [ObservableProperty] private bool isNewEmployee;
    [ObservableProperty] private string title;
    [ObservableProperty] private int employeeId;

    [ObservableProperty] private ObservableCollection<Department?> departments;
    [ObservableProperty] private ObservableCollection<string> genders;

    public EmployeeDetailViewModel(int empId)
    {
        _employeeService = new EmployeeService();
        _departmentService = new DepartmentService();

        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
        CancelCommand = new RelayCommand(ExecuteCancel);
        ChangePhotoCommand = new RelayCommand(ExecuteChangePhoto);

        EmployeeId = empId;
        Genders = new ObservableCollection<string> { "Male", "Female", "Other" };
        Initialize();
        LoadDepartments();
        IsEditable = UserSession.Instance.User.Role == "Admin";
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ChangePhotoCommand { get; }

    public void Initialize()
    {
        IsNewEmployee = EmployeeId <= 0;
        Title = IsNewEmployee ? "Add New Employee" : "Edit Employee";

        if (IsNewEmployee)
        {
            Employee = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Now),
            };
        }
        else
        {
            LoadEmployee();
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
            MessageBox.Show($"Failed to load employee: {ex.Message}","Error");
        }
    }

    private async void LoadDepartments()
    {
        try
        {
            var departments = await _departmentService.GetAllDepartments();
            Departments = new ObservableCollection<Department?>(departments);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load departments: {ex.Message}","Error");
        }
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
                await _employeeService.CreateEmployeeAsync(Employee);
            }
            else
            {
                await _employeeService.UpdateEmployeeAsync(EmployeeId, Employee);
            }
            MessageBox.Show(IsNewEmployee ? "Employee created successfully!" : "Employee updated successfully!", "Success");
            CloseWindow();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save employee: {ex.Message}", "Error");
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
            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
            Title = "Select Employee Photo"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string newPhotoPath = SavePhotoToStorage(openFileDialog.FileName);
                Employee.PhotoPath = newPhotoPath;
                if (await _employeeService.UploadAvatarAsync(EmployeeId, newPhotoPath))
                {
                    MessageBox.Show("Photo changed successfully!", "Success");
                }
                else
                {
                    MessageBox.Show("Failed to change photo", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( $"Failed to change photo: {ex.Message}", "Error");
            }
        }
    }

    private string SavePhotoToStorage(string sourceFilePath)
    {
        // Define the directory where photos will be stored
        string photoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "avatar");

        // Ensure the directory exists
        if (!Directory.Exists(photoDirectory))
        {
            Directory.CreateDirectory(photoDirectory);
        }

        // Generate a unique file name for the photo
        string fileName = Path.GetFileName(sourceFilePath);
        string destinationFilePath = Path.Combine(photoDirectory, fileName);

        // Copy the photo to the storage directory
        File.Copy(sourceFilePath, destinationFilePath, true);

        // Return the new file path
        return destinationFilePath;
    }

    private bool ValidateEmployee()
    {
        if (string.IsNullOrWhiteSpace(Employee.FullName))
        {
            MessageBox.Show( "Full name is required.", "Validation Error");
            return false;
        }

        if (Employee.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
        {
            MessageBox.Show("Birth date cannot be in the future.", "Validation Error");
            return false;
        }

        if (Employee.HireDate > DateOnly.FromDateTime(DateTime.Now))
        {
            MessageBox.Show( "Start date cannot be in the future.", "Validation Error");
            return false;
        }

        if (Employee.BasicSalary < 0)
        {
            MessageBox.Show("Salary cannot be negative.", "Validation Error");
            return false;
        }

        if (Employee.Department == null)
        {
            MessageBox.Show("Department is required.", "Validation Error");
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