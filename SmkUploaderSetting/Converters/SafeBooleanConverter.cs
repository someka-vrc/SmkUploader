using System.Globalization;
using System.Windows.Data;

namespace SmkUploaderSetting.Converters;

/// <summary>
/// Bool型の値のみを安全にバインドするためのコンバーター
/// </summary>
public class SafeBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }
        return false; // デフォルト値
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }
        return false; // デフォルト値
    }
}