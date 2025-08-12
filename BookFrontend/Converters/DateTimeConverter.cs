using System.Globalization;
using System.Windows.Data;

namespace book_frontend.Converters;

/// <summary>
/// 日期时间格式转换器
/// 用于在WPF中将DateTime对象转换为指定格式的字符串显示
/// </summary>
public class DateTimeConverter : IValueConverter
{
    /// <summary>
    /// 将DateTime转换为格式化字符串
    /// </summary>
    /// <param name="value">DateTime对象</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="parameter">格式化参数，如"yyyy-MM-dd"、"short"、"long"等</param>
    /// <param name="culture">文化信息</param>
    /// <returns>格式化的日期字符串</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime)
            return string.Empty;

        // 如果DateTime为默认值（最小值），显示空或占位符
        if (dateTime == DateTime.MinValue || dateTime == default(DateTime))
            return "未设置";

        string format = parameter?.ToString() ?? "yyyy-MM-dd HH:mm:ss";
        
        // 支持预定义格式
        return format.ToLower() switch
        {
            "short" => dateTime.ToString("yyyy-MM-dd", culture),
            "long" => dateTime.ToString("yyyy年MM月dd日 HH:mm:ss", culture),
            "date" => dateTime.ToString("yyyy-MM-dd", culture),
            "time" => dateTime.ToString("HH:mm:ss", culture),
            "datetime" => dateTime.ToString("yyyy-MM-dd HH:mm", culture),
            "chinese" => dateTime.ToString("yyyy年MM月dd日", culture),
            _ => dateTime.ToString(format, culture)
        };
    }

    /// <summary>
    /// 从字符串转回DateTime（用于双向绑定）
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            // 尝试多种日期格式解析
            string[] formats = {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd",
                "MM/dd/yyyy",
                "MM/dd/yyyy HH:mm:ss",
                "yyyy/MM/dd",
                "yyyy/MM/dd HH:mm:ss"
            };
            
            if (DateTime.TryParseExact(str, formats, culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            
            // 如果特定格式解析失败，使用通用解析
            if (DateTime.TryParse(str, culture, DateTimeStyles.None, out result))
            {
                return result;
            }
        }
        return DateTime.MinValue;
    }
}
