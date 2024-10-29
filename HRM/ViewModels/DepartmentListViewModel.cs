using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;

namespace HRM.ViewModels;

public partial class DepartmentListViewModel : ObservableObject
{
    private IDepartmentService _departmentService;
    public ObservableCollection<Department> Departments { get; set; }
    public Department? SelectedDepartment { get; set; }

    public ICommand AddDepartmentCommand { get; }
    public ICommand SaveDepartmentCommand { get; }

    public DepartmentListViewModel()
    {
        _departmentService = new DepartmentService();
        Departments = new ObservableCollection<Department>();
        
        // Initialize commands
        AddDepartmentCommand = new RelayCommand(AddDepartment);
        SaveDepartmentCommand = new RelayCommand(SaveDepartment, CanSave);

        // Load Departments (dummy data for demonstration)
        LoadDepartments();
    }

    private void LoadDepartments()
    {
        // This would typically pull from a data source
        Departments.Add(new Department { Name = "HR", Description = "Human Resources" });
        Departments.Add(new Department { Name = "IT", Description = "Information Technology" });
    }

    private void AddDepartment()
    {
        var newDepartment = new Department { Name = "New Department", Description = "Description" };
        Departments.Add(newDepartment);
        SelectedDepartment = newDepartment;
    }

    private void SaveDepartment()
    {
        Department department = new Department
        {
            Name = SelectedDepartment!.Name,
            Description = SelectedDepartment!.Description
            
        };
    }

    private bool CanSave() => SelectedDepartment != null;
}