using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;

namespace book_frontend.Helpers;

[Obsolete("没有用的代码")]
public class PasswordBoxHelper
{
    // 防止循环更新的标记
    private static bool _isUpdating;

    // 是否启用绑定的开关（附加属性）
    public static readonly DependencyProperty BindPasswordProperty =
        DependencyProperty.RegisterAttached("BindPassword", typeof(bool),
            typeof(PasswordBoxHelper), new PropertyMetadata(false, OnBindPasswordChanged));

    public static void SetBindPassword(DependencyObject dp, bool value) =>
        dp.SetValue(BindPasswordProperty, value);

    public static bool GetBindPassword(DependencyObject dp) =>
        (bool)dp.GetValue(BindPasswordProperty);

    // 绑定的密码字符串（附加属性）
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty,
                OnBoundPasswordChanged)
        );

    private static void SetBoundPassword(DependencyObject dp, string value) =>
        dp.SetValue(BoundPasswordProperty, value);

    public static string GetBoundPassword(DependencyObject dp) =>
        (string)dp.GetValue(BoundPasswordProperty);

    // 当BindPassword切换时，订阅/取消订阅Password Changed事件
    private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox passwordBox)
        {
            return;
        }

        bool wasBound = (bool)e.OldValue;
        bool needBind = (bool)e.NewValue;
        switch (wasBound)
        {
            case true when !needBind:
                passwordBox.PasswordChanged -= HandlePasswordChanged;
                break;
            case false when needBind:
                passwordBox.PasswordChanged += HandlePasswordChanged;
                break;
        }
    }

    // 当附加的 BoundPassword 变化时，更新到PasswordBox.Password
    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox passwordBox)
        {
            return;
        }

        if (!_isUpdating)
        {
            passwordBox.Password = e.NewValue as string ?? string.Empty;
        }
    }

    // 当用户在PasswordBox输入时，更新附加属性BoundPassword，从而同步到ViewModel
    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        _isUpdating = true;
        SetBoundPassword(passwordBox, passwordBox.Password);
        _isUpdating = false;
    }
}