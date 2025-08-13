using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 字符串到可见性转换器
/// 用于将字符串转换为Visibility枚举值
/// 空字符串或null时隐藏，有内容时显示
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 将字符串转换为Visibility
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数，如果为"Invert"则反转逻辑</param>
    /// <param name="culture">文化信息</param>
    /// <returns>Visibility枚举值</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hasValue = !string.IsNullOrEmpty(value?.ToString());
        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
        
        if (invert)
        {
            hasValue = !hasValue;
        }
            
        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 从Visibility转回字符串（用于双向绑定）
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
            return string.Empty;

        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) == true;
        var isVisible = visibility == Visibility.Visible;
        
        if (invert)
        {
            isVisible = !isVisible;
        }
            
        return isVisible ? "有内容" : string.Empty;
    }
}