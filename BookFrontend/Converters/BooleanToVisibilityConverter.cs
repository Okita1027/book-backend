using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 布尔值到可见性转换器
/// 用于在WPF中根据布尔条件控制元素的显示/隐藏
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 将布尔值转换为Visibility枚举
    /// </summary>
    /// <param name="value">源值（通常是bool或string）</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">转换参数，"invert"表示反转逻辑</param>
    /// <param name="culture">文化信息</param>
    /// <returns>Visibility.Visible 或 Visibility.Collapsed</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = value switch
        {
            bool b => b,
            string s => !string.IsNullOrEmpty(s),  // 字符串非空则显示
            int i => i > 0,                        // 数值大于0则显示
            null => false,
            _ => System.Convert.ToBoolean(value)   // 其他类型尝试转换为bool
        };
        
        // 检查是否需要反转逻辑
        bool shouldInvert = parameter?.ToString()?.ToLower() == "invert";
        if (shouldInvert)
        {
            isVisible = !isVisible;
        }
        
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 从Visibility转回布尔值（通常不需要）
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            bool shouldInvert = parameter?.ToString()?.ToLower() == "invert";
            return shouldInvert ? !result : result;
        }
        return false;
    }
}