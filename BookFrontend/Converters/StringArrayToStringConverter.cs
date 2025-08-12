using System.Globalization;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 字符串数组到字符串转换器
/// 用于将字符串数组转换为用逗号分隔的字符串显示
/// </summary>
public class StringArrayToStringConverter : IValueConverter
{
    /// <summary>
    /// 将字符串数组转换为逗号分隔的字符串
    /// </summary>
    /// <param name="value">字符串数组</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">分隔符参数，默认为逗号</param>
    /// <param name="culture">文化信息</param>
    /// <returns>逗号分隔的字符串</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case string[] { Length: > 0 } stringArray:
            {
                var separator = parameter?.ToString() ?? ", ";
                return string.Join(separator, stringArray);
            }
            case List<string> { Count: > 0 } stringList:
            {
                var separator = parameter?.ToString() ?? ", ";
                return string.Join(separator, stringList);
            }
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 从字符串转回字符串数组（用于双向绑定）
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str || string.IsNullOrEmpty(str)) return Array.Empty<string>();
        var separator = parameter?.ToString() ?? ", ";
        return str.Split([separator], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();
    }
}