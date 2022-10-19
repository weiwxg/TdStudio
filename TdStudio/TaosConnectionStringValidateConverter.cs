using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace TdStudio;

public sealed class TaosConnectionStringValidateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        const string pattern =
            @"^((ht|f)tps?:\/\/)?[\w-]+(\.[\w-]+)+:\d{1,5}\/?\?database=\w+&username=\w+&password=\w+$";
        return new Regex(pattern).IsMatch((string)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}