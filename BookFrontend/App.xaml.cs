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
    protected override void OnStartup(StartupEventArgs e)
    {
        // 创建主机构建器

    }

    
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}