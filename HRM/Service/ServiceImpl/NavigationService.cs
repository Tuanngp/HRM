﻿using HRM.Views;
using HRM.Views.Admin;
using HRM.Views.User;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HRM.Service.ServiceImpl;

public class NavigationService : INavigationService
{
    private readonly Frame _frame;
    private readonly Dictionary<string, Type> _pageMap;
    private object _parameter;

    public NavigationService(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        _frame.Navigated += Frame_Navigated;

        // Đăng ký các pages
        _pageMap = new Dictionary<string, Type>
        {
            { "LoginView", typeof(LoginView) },
            { "AdminDashboard", typeof(AdminDashboard) },
            { "UserDashboard", typeof(UserDashboard) },
            { "EmployeeListView", typeof(EmployeeListView) },
            { "EmployeeDetailView", typeof(EmployeeDetailView) },
            { "DepartmentListView", typeof(DepartmentListView) },
            { "DepartmentDetailView", typeof(DepartmentDetailView) },
            { "SalaryManagementView", typeof(SalaryManagementView)},
            {"TimekeepingManagementView", typeof(TimekeepingManagementView)},
            {"ReportView", typeof(ReportView)},
            {"UserProfileView", typeof(UserProfileView)}
        };
    }

    public bool CanGoBack => _frame.CanGoBack;

    public object Parameter => _parameter;

    public event EventHandler<NavigationEventArgs> Navigated;

    public void GoBack()
    {
        if (CanGoBack)
        {
            _frame.GoBack();
        }
    }

    public void NavigateTo(string viewName, object parameter = null)
    {
        if (!_pageMap.ContainsKey(viewName))
        {
            throw new ArgumentException($"Page {viewName} not found in page map.");
        }

        _parameter = parameter;
        var pageType = _pageMap[viewName];

        // Tạo instance của page
        var page = Activator.CreateInstance(pageType);

        // Thực hiện điều hướng
        _frame.Navigate(page, parameter);
    }

    public void NavigateToLogin()
    {
        NavigateTo("LoginView");
    }

private void Frame_Navigated(object sender, NavigationEventArgs e)
    {
        Navigated?.Invoke(this, e);
    }
}