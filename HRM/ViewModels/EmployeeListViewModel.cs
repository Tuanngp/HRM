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
using HRM.Service.ServiceImpl;
using HRM.Views;

namespace HRM.ViewModels;

public partial class EmployeeListViewModel : ObservableObject
{
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
    
    [ObservableProperty] private int selectedPageSize;
    [ObservableProperty] private int currentPage;
    [ObservableProperty] private int totalPages;
    

    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentRepository _departmentRepository;

    [ObservableProperty] public ObservableCollection<int> pageSizes;
    public ObservableCollection<Employee?> Employees { get; set; }
    public ObservableCollection<Department?> Departments { get; set; }
    public ObservableCollection<Gender> Genders { get; }
    public ObservableCollection<string> SalaryRanges { get; }

    public EmployeeListViewModel()
    {
        Employees = new ObservableCollection<Employee?>();
        Departments = new ObservableCollection<Department?>
        {
            new Department { Id = 0, Name = "All" }
        };
        Genders = new ObservableCollection<Gender> { Gender.Male , Gender.Female, Gender.Other};
        SalaryRanges = new ObservableCollection<string> { "< 5,000,000", "5,000,000 - 10,000,000", "> 10,000,000" };
        PageSizes = new ObservableCollection<int> { 10, 30, 50, 100 };

        _employeeService = new EmployeeService();
        _departmentRepository = new DepartmentRepository(new HrmContext());
        
        IsAdmin = UserSession.Instance.User!.Role == "Admin";
        _ = LoadData();
    }

    private async Task LoadData()
    {
        await LoadEmployees();
        await LoadDepartments();
        var employees = await _employeeService.GetAllEmployees();
        var empList = employees.ToList();
        
        TotalEmployees = empList.Count();
        ActiveEmployees = empList.Count(e => e!.Status == EmployeeStatus.Active);
        EmployeesOnLeave = empList.Count(e => e!.Status == EmployeeStatus.OnLeave);
        NewEmployees = empList.Count(e => e!.HireDate.Year == DateTime.Now.Year && e.HireDate.Month == DateTime.Now.Month);
        
        CurrentPage = 1;
        TotalPages = (int)Math.Ceiling((double)Employees.Count / SelectedPageSize);
    }

    private async Task LoadEmployees()
    {
        var employees = await _employeeService.GetAllEmployees();
        Employees.Clear();
        foreach (var e in employees)
        {
            Employees.Add(e);
        }
    }

    private async Task LoadDepartments()
    {
        var departments = await _departmentRepository.GetAllAsync();
        foreach (var d in departments)
        {
            Departments.Add(d);
        }
    }
    
    [RelayCommand]
    private void PreviousPage()
    {
        CurrentPage--;
    }  
    
    [RelayCommand]
    private void NextPage()
    {
        CurrentPage++;
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrEmpty(SearchText) && SelectedDepartment==null && !StartDate.HasValue && !EndDate.HasValue)
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
        var result = MessageBox.Show("Are you sure you want to delete this employee?", "Delete Employee", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            _employeeService.DeleteEmployeeAsync(employee!.Id);
            Employees.Remove(employee);
            MessageBox.Show("Deleted successfully", "Delete Employee", MessageBoxButton.OK);
        }
    }
    
    [RelayCommand]
    private async Task ExportToExcel()
    {
        System.Windows.Forms.MessageBox.Show("Exporting to Excel...");
        var exceData = await _employeeService.ExportToExcelAsync(Employees);
        var filePath = "employees.xlsx";
        System.Windows.Forms.MessageBox.Show($"Exported to {filePath}");
        await SaveExcelFileAsync(exceData, filePath);
        OpenExcelFile(filePath);
    }
    
    private string IsWithinSalaryRange()
    {
        if (SelectedSalaryRange == "< 5,000,000")
            return "0-4999999";
        if (SelectedSalaryRange == "5,000,000 - 10,000,0000")
            return "5000000-10000000";
        if (SelectedSalaryRange == "> 10,000,000")
            return "10000001-1000000000";
        return "";
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