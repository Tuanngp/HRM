using HRM.Models;

namespace HRM.Repositories;

public interface IAttendanceRepository : IBaseRepository<Attendance>
{
    Task<IEnumerable<Attendance>> GetAttendancesByDateAsync(DateTime? date, int employeeId);
    Task<Attendance> CheckInAsync(int employeeId);
    Task<Attendance> CheckOutAsync(int employeeId);
    Task<bool> HasCheckedInTodayAsync(int employeeId);
    Task ExportToExcelAsync(DateTime startDate, DateTime endDate, int employeeId);
    Task ExportToPdfAsync(DateTime startDate, DateTime endDate, int employeeId);
}