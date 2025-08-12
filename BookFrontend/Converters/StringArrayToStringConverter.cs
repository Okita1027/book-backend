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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string[] stringArray && stringArray.Length > 0)
        {
            string separator = parameter?.ToString() ?? ", ";
            return string.Join(separator, stringArray);
        }
        
        if (value is List<string> stringList && stringList.Count > 0)
        {
            string separator = parameter?.ToString() ?? ", ";
            return string.Join(separator, stringList);
        }
        
        return string.Empty;
    }

    /// <summary>
    /// 从字符串转回字符串数组（用于双向绑定）
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            string separator = parameter?.ToString() ?? ", ";
            return str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(s => s.Trim())
                     .ToArray();
        }
        
        return new string[0];
    }
}