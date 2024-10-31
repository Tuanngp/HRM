using HRM.Service.ServiceImpl;

namespace HRM.Models;

public class UserSession
{
    private IEmployeeService _employeeService = new EmployeeService();
    private static UserSession? _instance;
    public static UserSession Instance => _instance ??= new UserSession();

    public User? User { get; set; }
    public Employee? Employee { get; set; }
    public string? LastPageVisited { get; set; }

    private UserSession() { }

    public async Task SetUser(User? user)
    {
        User = user;
        Employee = await _employeeService.GetByUserId(User.Id);
    }

    public async Task<Employee> GetEmployee()
    {
        return Employee;
    }

    public void Clear()
    {
        User = null;
    }
}

public partial class SessionData
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string LastPage { get; set; }
}