using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 布尔值到可见性转换器
/// 用于将布尔值转换为Visibility枚举值
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 将布尔值转换为Visibility
    /// </summary>
    /// <param name="value">布尔值</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数，如果为"Invert"则反转逻辑</param>
    /// <param name="culture">文化信息</param>
    /// <returns>Visibility枚举值</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return Visibility.Collapsed;

        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
        
        if (invert)
            boolValue = !boolValue;
            
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 从Visibility转回布尔值（用于双向绑定）
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
            return false;

        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
        var result = visibility == Visibility.Visible;
        
        return invert ? !result : result;
    }
}