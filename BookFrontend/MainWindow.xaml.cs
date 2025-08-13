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
using book_frontend.Views.UserControls;
using book_frontend.Views;
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
        
        // 设置HomeView的DataContext
        if (MainContent.Content is HomeView homeView)
        {
            homeView.DataContext = mainViewModel.Home;
        }
    }

    /// <summary>
    /// 首页按钮点击事件
    /// </summary>
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        ShowHomeView();
    }

    /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLoginView();
    }

    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ShowRegisterView();
    }

    /// <summary>
    /// 显示首页视图
    /// </summary>
    private void ShowHomeView()
    {
        var homeView = new HomeView();
        if (DataContext is MainViewModel mainViewModel)
        {
            homeView.DataContext = mainViewModel.Home;
        }
        MainContent.Content = homeView;
    }

    /// <summary>
    /// 显示登录视图
    /// </summary>
    private void ShowLoginView()
    {
        var loginView = new LoginView();
        
        // 创建或重用LoginViewModel
        _loginViewModel ??= new LoginViewModel(_authService);
        
        loginView.DataContext = _loginViewModel;
        MainContent.Content = loginView;
    }

    /// <summary>
    /// 显示注册视图
    /// </summary>
    private void ShowRegisterView()
    {
        var registerView = new RegisterView();
        
        // 创建或重用RegisterViewModel
        if (_registerViewModel == null)
        {
            _registerViewModel = new RegisterViewModel(_authService);
            // 订阅导航到登录页面的事件
            _registerViewModel.NavigateToLogin += () => ShowLoginView();
        }
        
        // 重置表单状态
        _registerViewModel.ResetForm();
        
        registerView.DataContext = _registerViewModel;
        MainContent.Content = registerView;
    }
}