using System.Windows;
using System.Windows.Controls;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// MessageDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class MessageDisplay : UserControl
    {
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(MessageDisplay));

        public static readonly DependencyProperty SuccessMessageProperty =
            DependencyProperty.Register(nameof(SuccessMessage), typeof(string), typeof(MessageDisplay));

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        /// <summary>
        /// 成功消息
        /// </summary>
        public string SuccessMessage
        {
            get => (string)GetValue(SuccessMessageProperty);
            set => SetValue(SuccessMessageProperty, value);
        }

        public MessageDisplay()
        {
            InitializeComponent();
        }
    }
}