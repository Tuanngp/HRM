using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Models.Enum;
using HRM.Repositories;
using HRM.Repositories.RepositoryImpl;
using HRM.Service;
using HRM.Service.ServiceImpl;
using HRM.Views;

namespace HRM.ViewModels;

public partial class EmployeeListViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService = new EmployeeService();
    private readonly IDepartmentService _departmentService = new DepartmentService();
    
    [ObservableProperty] private string? searchText;
    [ObservableProperty] private Department? selectedDepartment;
    [ObservableProperty] private string? selectedGender;
    [ObservableProperty] private string? selectedSalaryRange;
    [ObservableProperty] private DateTime? startDate;
    [ObservableProperty] private DateTime? endDate;
    [ObservableProperty] private bool isAdmin;

    [ObservableProperty] private int totalEmployees;
    [ObservableProperty] private int activeEmployees;
    [ObservableProperty] private int employeesOnLeave;
    [ObservableProperty] private int newEmployees;

    [ObservableProperty] private int selectedPageSize = 10;
    [ObservableProperty] private int currentPage;
    [ObservableProperty] private int totalPages;
    
    [ObservableProperty] private ObservableCollection<int> pageSizes;
    [ObservableProperty] private ObservableCollection<Employee?> employees;
    [ObservableProperty] private ObservableCollection<Department?> departments;
    [ObservableProperty] private ObservableCollection<Gender> genders;
    [ObservableProperty] private ObservableCollection<string> salaryRanges;

    public EmployeeListViewModel()
    {
        IsAdmin = UserSession.Instance.User!.Role == "Admin";
        LoadCollections();
        _ = LoadData();
    }

    private void LoadCollections()
    {
        Employees = new ObservableCollection<Employee?>();
        Departments = new ObservableCollection<Department?>();
        Genders = new ObservableCollection<Gender> { Gender.Male , Gender.Female, Gender.Other};
        SalaryRanges = new ObservableCollection<string> { "< 5,000,000", "5,000,000 - 10,000,000", "> 10,000,000" };
        PageSizes = new ObservableCollection<int> { 10, 30, 50, 100 };
    }

    private async Task LoadData()
    {
        await LoadEmployees();
        await LoadDepartments();
        await LoadStatistic();
        LoadPagging();
    }

    private void LoadPagging()
    {
        CurrentPage = 1;
        TotalPages = (int)Math.Ceiling((double)Employees.Count / SelectedPageSize);
    }
    private async Task LoadStatistic()
    {
        var empEnumer = await _employeeService.GetAllEmployees();
        var empList = empEnumer.ToList();

        TotalEmployees = empList.Count();
        ActiveEmployees = empList.Count(e => e!.Status == EmployeeStatus.Active);
        EmployeesOnLeave = empList.Count(e => e!.Status == EmployeeStatus.OnLeave);
        NewEmployees = empList.Count(e => e!.HireDate.Year == DateTime.Now.Year && e.HireDate.Month == DateTime.Now.Month);
    }

    private async Task LoadEmployees()
    {
        var employeesInit = await _employeeService.GetAllEmployees();
        Employees.Clear();
        foreach (var e in employeesInit)
        {
            Employees.Add(e);
        }
    }

    private async Task LoadDepartments()
    {
        var departmentsInit = await _departmentService.GetAllDepartments();
        Departments.Clear();
        Departments.Add(new Department { Id = 0, Name = "Tất cả" });
        foreach (var d in departmentsInit)
        {
            Departments.Add(d);
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrEmpty(SearchText) && SelectedDepartment == null && !StartDate.HasValue && !EndDate.HasValue)
        {
            await LoadEmployees();
            return;
        }
        var searchResult = await _employeeService
            .SearchEmployeesAsync(SearchText, SelectedDepartment?.Id, StartDate, EndDate);
        Employees.Clear();
        foreach (var s in searchResult)
        {
            Employees.Add(s);
        }
    }

    [RelayCommand]
    private void AddEmployee()
    {
        OpenEmployeeDetailView(0);
    }

    [RelayCommand]
    private async Task ApplyFilters()
    {
        var filteredEmployees = await _employeeService.FilterEmployeesAsync(SelectedGender, IsWithinSalaryRange(), StartDate, EndDate);
        Employees.Clear();
        foreach (var e in filteredEmployees)
        {
            Employees.Add(e);
        }
    }

    [RelayCommand]
    private void ViewEmployee(Employee? employee)
    {
        OpenEmployeeDetailView(employee!.Id);
    }

    [RelayCommand]
    private void EditEmployee(Employee? employee)
    {
        OpenEmployeeDetailView(employee!.Id);
    }

    [RelayCommand]
    private void DeleteEmployee(Employee? employee)
    {
        var result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?", "Xóa Nhân Viên", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            _employeeService.DeleteEmployeeAsync(employee!.Id);
            Employees.Remove(employee);
            MessageBox.Show("Xóa thành công", "Xóa Nhân Viên", MessageBoxButton.OK);
        }
    }

    [RelayCommand]
    private async Task ExportToExcel()
    {
        // System.Windows.Forms.MessageBox.Show("Đang xuất ra Excel...");
        var excelData = await _employeeService.ExportToExcelAsync(Employees);
        var filePath = "employees.xlsx";
        System.Windows.Forms.MessageBox.Show($"Đã xuất tệp excel ra: {filePath}");
        await SaveExcelFileAsync(excelData, filePath);
        OpenExcelFile(filePath);
    }

    private string IsWithinSalaryRange()
    {
        return SelectedSalaryRange switch
        {
            "< 5,000,000" => "0-4999999",
            "5,000,000 - 10,000,000" => "5000000-10000000",
            "> 10,000,000" => "10000001-1000000000",
            _ => ""
        };
    }

    private void OpenEmployeeDetailView(int employeeId)
    {
        var employeeDetailView = new EmployeeDetailView(employeeId);
        employeeDetailView.Closed += EmployeeDetailView_Closed;
        employeeDetailView.Show();
    }

    private void EmployeeDetailView_Closed(object? sender, EventArgs e)
    {
        ((EmployeeDetailView)sender!).Closed -= EmployeeDetailView_Closed;
        _ = LoadData();
    }

    public async Task SaveExcelFileAsync(byte[] excelData, string filePath)
    {
        await File.WriteAllBytesAsync(filePath, excelData);
    }

    public void OpenExcelFile(string filePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            }
        };
        process.Start();
    }
}