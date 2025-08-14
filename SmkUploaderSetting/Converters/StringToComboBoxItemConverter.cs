using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SmkUploaderSetting.Converters;

/// <summary>
/// 文字列値に基づいてComboBoxの項目を選択するためのコンバーター
/// </summary>
public class StringToComboBoxItemConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem item)
        {
            return item.Content?.ToString() ?? string.Empty;
        }
        return value?.ToString() ?? string.Empty;
    }
}