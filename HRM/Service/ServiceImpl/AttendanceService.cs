﻿using HRM.Models;
using HRM.Repositories;
using HRM.Repositories.RepositoryImpl;
using Microsoft.EntityFrameworkCore;

namespace HRM.Service.ServiceImpl;

public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService()
        {
            _attendanceRepository = new AttendanceRepository(new HrmContext());
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByDateAsync(DateTime? date, int employeeId)
        {
            return await _attendanceRepository.GetAttendancesByDateAsync(date, employeeId);
        }

        public async Task<Attendance> CheckInAsync(int employeeId)
        {
            return await _attendanceRepository.CheckInAsync(employeeId);
        }

        public async Task<Attendance> CheckOutAsync(int employeeId)
        {
            return await _attendanceRepository.CheckOutAsync(employeeId);
        }

        public async Task<bool> HasCheckedInTodayAsync(int employeeId)
        {
            return await _attendanceRepository.HasCheckedInTodayAsync(employeeId);
        }

        public Task<Attendance?> GetByIdAsync(int id)
        {
            return _attendanceRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Attendance?>> GetAllAsync()
        {
            return _attendanceRepository.GetAllAsync();
        }

        public async Task<Attendance?> AddAsync(Attendance? attendance)
        {
            return await _attendanceRepository.AddAsync(attendance);
        }

        public Task UpdateAsync(Attendance? entity)
        {
            return _attendanceRepository.UpdateAsync(entity);
        }

        public Task DeleteAsync(int id)
        {
            return _attendanceRepository.DeleteAsync(id);
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