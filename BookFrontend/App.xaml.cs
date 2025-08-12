using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using book_frontend.Helpers;
using book_frontend.Services;
using book_frontend.Services.Interfaces;
using book_frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace book_frontend;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    protected override void OnStartup(StartupEventArgs e)
    {
        // 创建依赖注入容器
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        // 获取主窗口和主ViewModel
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

        // 设置主视图模型（其中包含首页 BookListViewModel）
        mainWindow.SetMainViewModel(mainViewModel);

        // 指定应用的主窗口并显示
        MainWindow = mainWindow;
        mainWindow.Show();

        base.OnStartup(e);
    }

    /// <summary>
    /// 配置依赖注入服务
    /// </summary>
    /// <param name="services">服务集合</param>
    private static void ConfigureServices(IServiceCollection services)
    {
        // 注册配置
        var config = ConfigurationHelper.GetConfig();

        // 注册HttpClient(单例)
        services.AddSingleton<HttpClient>(_ =>
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(config.ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.Timeout = TimeSpan.FromSeconds(config.RequestTimeoutSeconds);
            return httpClient;
        });
        // 注册ApiClient(单例)
        services.AddSingleton<ApiClient>();
        // 注册服务层(单例)
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IBookService, BookService>();
        services.AddSingleton<IUserService, UserService>();
        // 注册ViewModels(瞬时)
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<MainViewModel>();
        // 注册主窗口(瞬时)
        services.AddTransient<MainWindow>();
    }

    
    protected override void OnExit(ExitEventArgs e)
    {
        // 释放依赖注入容器资源
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}