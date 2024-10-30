﻿using HRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HRM.Repositories.RepositoryImpl;

public class AttendanceRepository(HrmContext context) : BaseRepository<Attendance>(context), IAttendanceRepository
{
    public IQueryable<Attendance?> AddIncludes(IQueryable<Attendance?> query)
    {
        return query.Include(a => a.Employee);
    }

    public async Task<IEnumerable<Attendance>> GetAttendancesByDateAsync(DateTime? date, int employeeId)
    {
        if (!date.HasValue)
        {
            return await context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId)
                .ToListAsync();
        }
        else
        {
            return await context.Attendances
                        .Include(a => a.Employee)
                        .Where(a => a.EmployeeId == employeeId && date != null && a.CheckInTime.Date == date.Value.Date)
                        .ToListAsync();
        }
        
        
    }

    public async Task<Attendance> CheckInAsync(int employeeId)
    {
        var attendance = new Attendance
        {
            EmployeeId = employeeId,
            CheckInTime = DateTime.Now,
            Status = "Present"
        };

        context.Attendances.Add(attendance);
        await context.SaveChangesAsync();
        return attendance;
    }

    public async Task<Attendance> CheckOutAsync(int employeeId)
    {
        var attendance = await context.Attendances
            .Where(a => a.EmployeeId == employeeId && a.CheckInTime.Date == DateTime.Today)
            .OrderByDescending(a => a.CheckInTime)
            .FirstOrDefaultAsync();

        if (attendance != null)
        {
            attendance.CheckOutTime = DateTime.Now;
            attendance.WorkingHours = (decimal)((DateTime)attendance.CheckOutTime - attendance.CheckInTime).TotalHours;
            await context.SaveChangesAsync();
        }

        return attendance;
    }

    public async Task<bool> HasCheckedInTodayAsync(int employeeId)
    {
        return await context.Attendances
            .AnyAsync(a => a.EmployeeId == employeeId &&
                           a.CheckInTime.Date == DateTime.Today &&
                           a.CheckOutTime == null);
    }

    public async Task ExportToExcelAsync(DateTime startDate, DateTime endDate, int employeeId)
    {
        // Implement Excel export logic
        await Task.CompletedTask;
    }

    public async Task ExportToPdfAsync(DateTime startDate, DateTime endDate, int employeeId)
    {
        // Implement PDF export logic
        await Task.CompletedTask;
    }
}