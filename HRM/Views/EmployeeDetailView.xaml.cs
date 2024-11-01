using System.Windows;
using System.Windows.Controls;
using HRM.ViewModels;

namespace HRM.Views;

public partial class EmployeeDetailView : Window
{
    public EmployeeDetailView(int employeeId, bool isEdit)
    {
        InitializeComponent();
        DataContext = new EmployeeDetailViewModel(employeeId, isEdit);
    }
}