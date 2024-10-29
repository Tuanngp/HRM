using System.Collections.ObjectModel;
using HRM.Models;
using HRM.Repositories;
using HRM.Repositories.RepositoryImpl;

namespace HRM.Service.ServiceImpl;

public class DepartmentService : IDepartmentService
{
    private IDepartmentRepository _departmentRepository;
    
    public DepartmentService()
    {
        _departmentRepository = new DepartmentRepository(new HrmContext());
    }
    
    public Task<ObservableCollection<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AssignEmployeeToDepartmentAsync(int employeeId, int departmentId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Department?>> GetAllDepartments()
    {
        return await _departmentRepository.GetAllAsync();
    }
}