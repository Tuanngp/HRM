using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRM.Models;
using HRM.Service;
using HRM.Service.ServiceImpl;

namespace HRM.ViewModels;

public partial class EmployeeCalendarViewModel : ObservableObject
    {
        private readonly IAttendanceService _attendanceService;

        public EmployeeCalendarViewModel()
        {
            _attendanceService = new AttendanceService();
            Attendances = new ObservableCollection<Attendance>();
            _ = LoadAttendances();
            // LoadEvents();
        }
        // [ObservableProperty]
        // private ObservableCollection<EventViewModel> events;

        [ObservableProperty]
        private ObservableCollection<Attendance> attendances;
        
        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool hasCheckIn;

        [ObservableProperty]
        private Attendance attendance;
        
        [RelayCommand]
        private void CheckIn()
        {
            if (UserSession.Instance.Employee != null)
                Attendance = new Attendance
                {
                    EmployeeId = UserSession.Instance.Employee.Id,
                    CheckInTime = DateTime.Now,
                    Status = "Checked In"
                };

            HasCheckIn = true;
            
            _attendanceService.AddAsync(Attendance);
        }
        
        // [RelayCommand]
        // private void AddEvent()
        // {
        //     var newEvent = new EventViewModel
        //     {
        //         Title = "New Event",
        //         Time = "10:00 AM - 11:00 AM",
        //         Location = "Meeting Room A",
        //         EventColor = System.Windows.Media.Brushes.Blue
        //     };
        //     Events.Add(newEvent);
        // }

        // private void LoadEvents()
        // {
        //     Events = new ObservableCollection<EventViewModel>
        //     {
        //         new EventViewModel
        //         {
        //             Title = "Department Meeting",
        //             Time = "09:00 AM - 10:30 AM",
        //             Location = "Room A",
        //             EventColor = System.Windows.Media.Brushes.Green
        //         },
        //         new EventViewModel
        //         {
        //             Title = "Training Session",
        //             Time = "14:00 PM - 16:00 PM",
        //             Location = "Training Room",
        //             EventColor = System.Windows.Media.Brushes.Red
        //         }
        //     };
        // }

        private async Task LoadAttendances()
        {
            if (UserSession.Instance.Employee != null)
            {
                var attendancesList = await _attendanceService
                    .GetAttendancesByDateAsync(null, UserSession.Instance.Employee.Id);
                foreach (var a in attendancesList)
                {
                    Attendances.Add(a);
                }
            }
        }
        
        public bool HasAttendanceOnDate(DateTime date)
        {
            try
            {
                return Attendances.Any(a => a.CheckInTime.Date == date.Date);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }