using System.Windows;
using System.Windows.Controls;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// LoginFormView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginFormView : UserControl
    {
        public static readonly DependencyProperty FooterContentProperty =
            DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(LoginFormView));

        /// <summary>
        /// 底部内容（用于放置导航链接等）
        /// </summary>
        public object FooterContent
        {
            get => GetValue(FooterContentProperty);
            set => SetValue(FooterContentProperty, value);
        }

        public LoginFormView()
        {
            InitializeComponent();
        }
    }
}