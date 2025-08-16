using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using book_frontend.ViewModels;
using book_frontend.Views.Pages;
using Hardcodet.Wpf.TaskbarNotification;
using book_frontend.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Application = System.Windows.Application;

namespace book_frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAuthService _authService;
    private readonly IBookService _bookService;
    private LoginViewModel? _loginViewModel;
    private RegisterViewModel? _registerViewModel;
    
    public ICommand ShowWindowCommand { get; set; }
    public ICommand ExitApplicationCommand { get; set; }
    
    public MainWindow(IAuthService authService, IBookService bookService)
    {
        InitializeComponent();
        _authService = authService;
        _bookService = bookService;
        Closing += Window_Closing;
        ShowWindowCommand = new RelayCommand(ShowWindow);
        ExitApplicationCommand = new RelayCommand(ExitApplication);
        
        /*
         * 不要在这里调用 ShowHomePage()，因为此时 DataContext 还未通过依赖注入设置
         * DataContext是当前UI元素绑定数据的"上下文"/"数据源"，它的作用是：
         *  - 数据绑定源：当XAML中的绑定表达式没有明确指定Source时，默认会从DataContext中获取数据
         *  - 继承性：DataContext具有继承性，子元素会继承父元素的DataContext，除非显式设置了自己的DataContext
         *  - MVVM模式基础：ViewModel通常被设置为View的DataContext，实现视图和数据的分离
         */
    }

    /// <summary>
    /// 窗口正在关闭时触发的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        // 取消关闭操作，并隐藏窗口
        e.Cancel = true;
        Hide();
    }

    /// <summary>
    /// 托盘菜单“打开”选项的执行方法
    /// </summary>
    private void ShowWindow()
    {
        // 激活窗口并将其带到最前面
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    /// <summary>
    /// 托盘菜单“退出”选项的执行方法
    /// </summary>
    private static void ExitApplication()
    {
        // 真正退出程序
        Application.Current.Shutdown();
    }
    

    /// <summary>
    /// 设置主ViewModel的DataContext
    /// </summary>
    /// <param name="mainViewModel">主视图模型</param>
    public void SetMainViewModel(MainViewModel mainViewModel)
    {
        DataContext = mainViewModel;
        
        // 如果当前内容是 HomePage，设置其 DataContext；否则主动导航到首页
        if (MainContentFrame?.Content is HomePage homePage)
        {
            homePage.DataContext = mainViewModel.Home;
        }
        else
        {
            ShowHomePage();
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
        var homePage = new HomePage(_bookService);
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
        if (_loginViewModel == null)
        {
            _loginViewModel = new LoginViewModel(_authService);
            // 订阅导航到注册页面的事件
            _loginViewModel.NavigateToRegister += ShowRegisterPage;
        }
        
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
            _registerViewModel.NavigateToLogin += ShowLoginPage;
        }
        
        // 重置表单状态
        _registerViewModel.ResetForm();
        
        registerPage.DataContext = _registerViewModel;
        MainContentFrame.Navigate(registerPage);
    }
}