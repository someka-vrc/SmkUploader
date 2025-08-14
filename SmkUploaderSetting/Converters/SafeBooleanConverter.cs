using System.Globalization;
using System.Windows.Data;

namespace SmkUploaderSetting.Converters;

/// <summary>
/// Bool�^�̒l�݂̂����S�Ƀo�C���h���邽�߂̃R���o�[�^�[
/// </summary>
public class SafeBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }
        return false; // �f�t�H���g�l
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }
        return false; // �f�t�H���g�l
    }
}