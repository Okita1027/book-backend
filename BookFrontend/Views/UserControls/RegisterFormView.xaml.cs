using System.Windows;
using System.Windows.Controls;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// RegisterFormView.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterFormView : UserControl
    {
        public static readonly DependencyProperty FooterContentProperty =
            DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(RegisterFormView));

        /// <summary>
        /// 底部内容（用于放置导航链接等）
        /// </summary>
        public object FooterContent
        {
            get => GetValue(FooterContentProperty);
            set => SetValue(FooterContentProperty, value);
        }

        public RegisterFormView()
        {
            InitializeComponent();
        }
    }
}