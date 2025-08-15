using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using book_frontend.Helpers;
using book_frontend.Services;
using book_frontend.Services.Interfaces;
using book_frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace book_frontend;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        // 确保日志目录存在
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 配置Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                standardErrorFromLevel: LogEventLevel.Error,
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
            )
            .WriteTo.File(
                Path.Combine("Logs", "log-.json"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                encoding: Encoding.UTF8
            )
            .CreateLogger();

        // 全局异常处理
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

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

        Log.Information("主窗口启动了……");
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

    /// <summary>
    /// 捕获UI线程（主线程）中未处理的异常，如按钮点击事件、数据绑定等操作中的异常。
    /// </summary>
    /// <param name="sender">应用程序实例</param>
    /// <param name="e">包含异常信息的事件参数</param>
    private static void App_DispatcherUnhandledException(object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "UI线程未处理异常");
        MessageBox.Show("发生未预期的错误，请查看日志了解详情。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        // 标记异常已被处理，防止应用程序崩溃
        e.Handled = true;
    }

    /// <summary>
    /// 捕获非UI线程中的未处理异常，这类异常通常会导致应用程序终止。
    /// </summary>
    /// <param name="sender">应用程序实例</param>
    /// <param name="e">包含异常信息的事件参数</param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.ExceptionObject as Exception, "非UI线程未处理异常");
        MessageBox.Show("发生严重错误，应用程序将关闭。", "严重错误", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// 捕获未观察到的任务异常，这些异常通常是在异步任务中未处理的异常。
    /// </summary>
    /// <param name="sender">任务调度器实例</param>
    /// <param name="e">包含异常信息的事件参数</param>
    /// <remarks>
    /// 当异步任务抛出异常但未被观察到时，会触发此事件。
    /// 可以在事件处理程序中记录异常信息，防止应用程序崩溃。
    /// </remarks>
    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "未观察到的任务异常");
        e.SetObserved();
    }


    protected override void OnExit(ExitEventArgs e)
    {
        // 释放依赖注入容器资源
        _serviceProvider?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}