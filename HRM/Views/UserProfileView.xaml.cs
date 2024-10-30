using System.Windows.Controls;
using HRM.ViewModels;

namespace HRM.Views;

public partial class UserProfileView : Page
{
    public UserProfileView()
    {
        InitializeComponent();
        DataContext = new UserProfileViewModel();
    }
}