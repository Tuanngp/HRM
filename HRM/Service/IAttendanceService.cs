using HRM.Models;
using HRM.Repositories;
using HRM.Service.ServiceImpl;

namespace HRM.Service;

public interface IAttendanceService : IBaseRepository<Attendance>
{
    
    Task<IEnumerable<Attendance>> GetAttendancesByDateAsync(DateTime? date, int employeeId);
    Task<Attendance> CheckInAsync(int employeeId);
    Task<Attendance> CheckOutAsync(int employeeId);
    Task<bool> HasCheckedInTodayAsync(int employeeId);
    Task ExportToExcelAsync(DateTime startDate, DateTime endDate, int employeeId);
    Task ExportToPdfAsync(DateTime startDate, DateTime endDate, int employeeId);
}