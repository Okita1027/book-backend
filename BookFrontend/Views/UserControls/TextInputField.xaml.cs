using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// TextInputField.xaml 的交互逻辑
    /// </summary>
    public partial class TextInputField : UserControl
    {
        /*
         * DependencyProperty.Register：注册依赖属性
         *  nameof(Text)：属性名"Text"
         *  typeof(string)：属性类型为string
         *  typeof(TextInputField)：所属类型
         *  FrameworkPropertyMetadata：元数据
         *  string.Empty：默认值
         *  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault：默认双向绑定
         */
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextInputField),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register(nameof(HintText), typeof(string), typeof(TextInputField));

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(PackIconKind), typeof(TextInputField));

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(TextInputField),
                new PropertyMetadata(true));

        /// <summary>
        /// 输入文本
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        /// <summary>
        /// 图标类型
        /// </summary>
        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        /// <summary>
        /// 是否显示图标
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public TextInputField()
        {
            InitializeComponent();
        }
    }
}