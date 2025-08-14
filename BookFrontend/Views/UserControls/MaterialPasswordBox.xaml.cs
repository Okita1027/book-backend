using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace book_frontend.Views.UserControls
{
    public partial class MaterialPasswordBox : UserControl
    {
        public MaterialPasswordBox()
        {
            InitializeComponent();
        }

        // 提示文字
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register(nameof(HintText), typeof(string), typeof(MaterialPasswordBox), new PropertyMetadata(string.Empty));
        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        // 图标类型
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(PackIconKind), typeof(MaterialPasswordBox), new PropertyMetadata(PackIconKind.Lock));
        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        // 密码（双向绑定）
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(MaterialPasswordBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }
    }
}