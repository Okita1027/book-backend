using System;
using System.Globalization;
using System.Windows.Data;

namespace book_frontend.Converters
{
    /// <summary>
    /// 编辑模式到保存按钮文本转换器
    /// true（编辑模式）转换为"更新"，false（新增模式）转换为"保存"
    /// </summary>
    public class EditModeToSaveTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditMode)
            {
                return isEditMode ? "更新" : "保存";
            }
            return "保存";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}