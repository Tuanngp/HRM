using System.Windows.Controls;
using HRM.ViewModels;

namespace HRM.Views;

public partial class EmployeeCalendarView : Page
{
    public EmployeeCalendarView()
    {
        InitializeComponent();
        DataContext = new EmployeeCalendarViewModel();
    }
}