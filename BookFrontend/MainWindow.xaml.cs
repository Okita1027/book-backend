using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using book_frontend.ViewModels;
using book_frontend.Views.Pages;
using book_frontend.Services.Interfaces;

namespace book_frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAuthService _authService;
    private LoginViewModel? _loginViewModel;
    private RegisterViewModel? _registerViewModel;
    
    public MainWindow(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    /// <summary>
    /// 设置主ViewModel的DataContext
    /// </summary>
    /// <param name="mainViewModel">主视图模型</param>
    public void SetMainViewModel(MainViewModel mainViewModel)
    {
        DataContext = mainViewModel;
        
        // 设置HomePage的DataContext
        if (MainContentFrame?.Content is HomePage homePage)
        {
            homePage.DataContext = mainViewModel.Home;
        }
    }

    /// <summary>
    /// 首页按钮点击事件
    /// </summary>
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        ShowHomePage();
    }

    /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLoginPage();
    }

    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ShowRegisterPage();
    }

    /// <summary>
    /// 显示首页Page
    /// </summary>
    private void ShowHomePage()
    {
        var homePage = new HomePage();
        if (DataContext is MainViewModel mainViewModel)
        {
            homePage.DataContext = mainViewModel.Home;
        }
        MainContentFrame.Navigate(homePage);
    }

    /// <summary>
    /// 显示登录Page
    /// </summary>
    private void ShowLoginPage()
    {
        var loginPage = new LoginPage();
        
        // 创建或重用LoginViewModel
        _loginViewModel ??= new LoginViewModel(_authService);
        
        loginPage.DataContext = _loginViewModel;
        MainContentFrame.Navigate(loginPage);
    }

    /// <summary>
    /// 显示注册Page
    /// </summary>
    private void ShowRegisterPage()
    {
        var registerPage = new RegisterPage();
        
        // 创建或重用RegisterViewModel
        if (_registerViewModel == null)
        {
            _registerViewModel = new RegisterViewModel(_authService);
            // 订阅导航到登录页面的事件
            _registerViewModel.NavigateToLogin += () => ShowLoginPage();
        }
        
        // 重置表单状态
        _registerViewModel.ResetForm();
        
        registerPage.DataContext = _registerViewModel;
        MainContentFrame.Navigate(registerPage);
    }
}