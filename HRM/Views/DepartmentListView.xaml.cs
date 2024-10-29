using System.Windows.Controls;
using HRM.ViewModels;

namespace HRM.Views.Admin;

public partial class DepartmentListView : Page
{
    public DepartmentListView()
    {
        InitializeComponent();
        DataContext = new DepartmentListViewModel();
    }
}