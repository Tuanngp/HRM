using System.Globalization;
using System.Windows.Data;
using HRM.ViewModels;

namespace HRM.Converter;

public class DateAndViewModelToAttendanceConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is DateTime date && values[1] is EmployeeCalendarViewModel viewModel)
        {
            return viewModel.HasAttendanceOnDate(date);
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}