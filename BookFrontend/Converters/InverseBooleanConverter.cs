using System.Globalization;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 反转布尔值转换器
/// 用于将布尔值进行反转操作
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// 将布尔值进行反转
    /// </summary>
    /// <param name="value">布尔值</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">参数（未使用）</param>
    /// <param name="culture">文化信息</param>
    /// <returns>反转后的布尔值</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return true; // 默认返回true
    }

    /// <summary>
    /// 反向转换,从目标到源（用于双向绑定）
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
            
        return false; // 默认返回false
    }
}