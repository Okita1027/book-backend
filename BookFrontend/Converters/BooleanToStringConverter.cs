using System;
using System.Globalization;
using System.Windows.Data;

namespace book_frontend.Converters;

public class BooleanToStringConverter : IValueConverter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">这是要转换的源值，即绑定源属性的值。</param>
    /// <param name="targetType">这是目标类型，即绑定目标属性的类型。</param>
    /// <param name="parameter">这是转换器参数，允许在XAML中向转换器传递额外的信息。</param>
    /// <param name="culture">这是当前的文化信息，用于本地化和区域性相关的转换。</param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string parameterString)
        {
            var options = parameterString.Split('|');
            if (options.Length == 2)
            {
                return boolValue ? options[0] : options[1];
            }
        }
        
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}