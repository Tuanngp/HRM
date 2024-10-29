using System.Collections.ObjectModel;
using HRM.Models;

namespace HRM.Service;

public interface IDepartmentService
{
    Task<ObservableCollection<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<bool> AssignEmployeeToDepartmentAsync(int employeeId, int departmentId);
    Task<IEnumerable<Department?>> GetAllDepartments();
}