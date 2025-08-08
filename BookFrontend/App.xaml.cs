using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace book_frontend;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;
    protected override void OnStartup(StartupEventArgs e)
    {
        // 配置依赖注入容器
        ServiceCollection serviceCollection = new ServiceCollection();
        
        
        // 启动登录窗口
        
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // HTTP 客户端配置
        
        // 注册服务
        
        // 注册ViewModels
        
        //注册Views
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}