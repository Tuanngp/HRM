using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Microsoft.EntityFrameworkCore;

namespace HRM.ViewModels;

public partial class EmployeeListViewModel : ObservableObject
{
    [ObservableProperty] private string searchText;
    [ObservableProperty] private Department selectedDepartment;
    [ObservableProperty] private string selectedGender;
    [ObservableProperty] private string selectedSalaryRange;
    [ObservableProperty] private DateTime? startDate;
    [ObservableProperty] private DateTime? endDate;
    [ObservableProperty] private bool isAdmin;

    private IEmployeeService _employeeService;
    private IDepartmentRepository _departmentRepository;

    public ObservableCollection<Employee?> Employees { get; set; }
    public ObservableCollection<Department?> Departments { get; set; }
    public ObservableCollection<Gender> Genders { get; }
    public ObservableCollection<string> SalaryRanges { get; }

    public ICommand SearchCommand { get; }
    public ICommand AddEmployeeCommand { get; }
    public ICommand ApplyFiltersCommand { get; }
    public ICommand ViewEmployeeCommand { get; }
    public ICommand EditEmployeeCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }

    public EmployeeListViewModel()
    {
        // Initialize collections and commands
        Employees = new ObservableCollection<Employee?>();
        Departments = new ObservableCollection<Department?>();
        Genders = new ObservableCollection<Gender>();
        SalaryRanges = new ObservableCollection<string> { "< 5,000", "5,000 - 10,000", "> 10,000" };

        // Initialize commands
        SearchCommand = new RelayCommand(OnSearch);
        AddEmployeeCommand = new RelayCommand(OnAddEmployee);
        ApplyFiltersCommand = new RelayCommand(OnApplyFilters);
        ViewEmployeeCommand = new RelayCommand<Employee>(OnViewEmployee);
        EditEmployeeCommand = new RelayCommand<Employee>(OnEditEmployee, CanEditOrDelete);
        DeleteEmployeeCommand = new RelayCommand<Employee>(OnDeleteEmployee, CanEditOrDelete);

        _employeeService = new EmployeeService();
        _departmentRepository = new DepartmentRepository(new HrmContext());
        
        IsAdmin = UserSession.Instance.User.Role == "Admin";
        LoadData();
    }

    private async void LoadData()
    {
        var employees = await _employeeService.GetAllEmployees();
        var departments = await _departmentRepository.GetAllAsync();
        Employees.Clear();
        Departments.Clear();
        foreach (var e in employees)
        {
            Employees.Add(e);
        }

        foreach (var d in departments)
        {
            Departments.Add(d);
        }
    }

    private async void OnSearch()
    {
        var searchResult = await _employeeService
            .SearchEmployeesAsync(SearchText, SelectedDepartment.Id, StartDate, EndDate);
        Employees.Clear();
        foreach (var s in searchResult)
        {
            Employees.Add(s);
        }
    }

    private void OnAddEmployee()
    {
    }

    private async void OnApplyFilters()
    {
        var filteredEmployees = await _employeeService.FilterEmployeesAsync(SelectedGender, IsWithinSalaryRange(), StartDate, EndDate);
        Employees.Clear();
        foreach (var e in filteredEmployees)
        {
            Employees.Add(e);
        }
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
    
    private void OnViewEmployee(Employee employee)
    {
        OpenEmployeeDetailView(employee.Id);
    }

    private void OnEditEmployee(Employee employee)
    {
        OpenEmployeeDetailView(employee.Id);
    }

    private void OpenEmployeeDetailView(int employeeId)
    {
        var employeeDetailView = new EmployeeDetailView(employeeId);
        employeeDetailView.Closed += EmployeeDetailView_Closed;
        employeeDetailView.Show();
    }

    private void EmployeeDetailView_Closed(object sender, EventArgs e)
    {
        ((EmployeeDetailView)sender).Closed -= EmployeeDetailView_Closed;
        LoadData();
    }

    private void OnDeleteEmployee(Employee employee)
    {
        var result = MessageBox.Show("Are you sure you want to delete this employee?", "Delete Employee", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            _employeeService.DeleteEmployeeAsync(employee.Id);
            Employees.Remove(employee);
            MessageBox.Show("Deleted successfully", "Delete Employee", MessageBoxButton.OK);
        }
    }

    private bool CanEditOrDelete(Employee employee)
    {
        return IsAdmin;
    }
}