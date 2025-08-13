using System.Windows;
using System.Windows.Controls;
using book_frontend.ViewModels;

namespace book_frontend.Views.UserControls;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 立即登录按钮点击事件
    /// </summary>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // 触发导航到登录页面的事件
        if (DataContext is RegisterViewModel viewModel)
        {
            viewModel.TriggerNavigateToLogin();
        }
    }
}