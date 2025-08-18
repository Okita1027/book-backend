using System.Windows;
using System.Windows.Controls;
using book_frontend.ViewModels;
using book_frontend.Views.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace book_frontend.Views.Pages;

/// <summary>
/// AdminPage.xaml 的交互逻辑
/// </summary>
public partial class AdminPage : Page
{
    private readonly IServiceProvider _serviceProvider;
    
    public AdminPage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        
        // 默认显示欢迎页面
        ShowWelcomePage();
    }

    /// <summary>
    /// 图书管理按钮点击事件
    /// </summary>
    private void BookManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ShowBookManagement();
        UpdateCurrentPageTitle("图书管理");
    }

    /// <summary>
    /// 用户管理按钮点击事件
    /// </summary>
    private void UserManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ShowUserManagement();
        UpdateCurrentPageTitle("用户管理");
    }



    /// <summary>
    /// 显示图书管理页面
    /// </summary>
    private void ShowBookManagement()
    {
        var bookManagementControl = _serviceProvider.GetRequiredService<BookManagement>();
        AdminContentFrame.Navigate(bookManagementControl);
    }

    /// <summary>
    /// 显示欢迎页面
    /// </summary>
    private void ShowWelcomePage()
    {
        var welcomeText = new TextBlock
        {
            Text = "欢迎使用后台管理系统",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            Foreground = System.Windows.Media.Brushes.Gray
        };
        
        AdminContentFrame.Navigate(welcomeText);
        UpdateCurrentPageTitle("欢迎");
    }

    /// <summary>
    /// 显示用户管理页面（暂时显示占位内容）
    /// </summary>
    private void ShowUserManagement()
    {
        var placeholder = new TextBlock
        {
            Text = "用户管理功能正在开发中...",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 16,
            Foreground = System.Windows.Media.Brushes.Gray
        };
        
        AdminContentFrame.Navigate(placeholder);
    }



    /// <summary>
    /// 更新当前页面标题
    /// </summary>
    private void UpdateCurrentPageTitle(string title)
    {
        if (DataContext is AdminViewModel adminViewModel)
        {
            adminViewModel.CurrentPageTitle = title;
        }
    }
}