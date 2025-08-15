using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace book_frontend.Controls
{
    public class PasswordField : Control
    {
        static PasswordField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordField),
                new FrameworkPropertyMetadata(typeof(PasswordField)));
        }

        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register(nameof(HintText), typeof(string), typeof(PasswordField),
                new PropertyMetadata(string.Empty));

        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(PackIconKind), typeof(PasswordField),
                new PropertyMetadata(PackIconKind.Lock));

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password), typeof(string), typeof(PasswordField),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }
    }
}