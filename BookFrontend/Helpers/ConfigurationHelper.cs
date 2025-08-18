using System.IO;
using System.Text.Json;

namespace book_frontend.Helpers;

public static class ConfigurationHelper
{
    private static AppConfig? _config;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // 大小写不敏感 + 驼峰命名规则
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    /// <summary>
    /// 应用配置模型
    /// </summary>
    public class AppConfig
    {
        // 优先从配置文件中读取
        public string ApiBaseUrl { get; init; } = "";
        public int RequestTimeoutSeconds { get; init; } = 30;
    }

    /// <summary>
    /// 获取应用配置
    /// 如果配置文件不存在，则返回默认配置
    /// </summary>
    /// <returns>应用配置对象</returns>
    public static AppConfig GetConfig()
    {
        if (_config != null)
        {
            return _config;
        }
        try
        {
            /*
             * 优先读取配置文件
             *  AppDomain
             *      AppDomain（应用程序域）是.NET中的一种轻量级隔离机制
             *      它是.NET应用程序运行的环境边界
             *      每个.NET应用程序至少运行在一个AppDomain中
             *  CurrentDomain
             *      CurrentDomain是AppDomain类的静态属性
             *      它返回当前线程正在运行的AppDomain实例
             *      通过这个属性可以访问当前应用程序域的各种信息
             *  BaseDirectory
             *      BaseDirectory是AppDomain实例的一个属性
             *      它返回应用程序的基目录路径
             */
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var parsed = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
                if (parsed != null)
                {
                    _config = parsed;
                }
            }
        }
        catch (Exception ex)
        {
            // 配置文件读取失败时，使用默认设置
            System.Diagnostics.Debug.WriteLine($"配置文件读取失败，使用默认配置:{ex.Message}");
        }
        // 如果配置为空或读取失败，创建默认配置（提供一个合理的默认值，避免空地址）
        _config ??= new AppConfig
        {
            ApiBaseUrl = "http://localhost:8888/api/",
            RequestTimeoutSeconds = 30
        };
        return _config;
    }
    
}
