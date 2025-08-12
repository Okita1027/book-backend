using System.IO;
using System.Text.Json;

namespace book_frontend.Helpers;

public static class ConfigurationHelper
{
    private static AppConfig? _config;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    /// <summary>
    /// 应用配置模型
    /// </summary>
    public class AppConfig
    {
        public string ApiBaseUrl { get; init; } = "http://localhost:8888/api/";
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
            // 查找配置文件路径
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
            }
        }
        catch (Exception ex)
        {
            // 配置文件读取失败时，使用默认设置
            System.Diagnostics.Debug.WriteLine($"配置文件读取失败，使用默认配置:{ex.Message}");
        }
        // 如果配置为空或读取失败，创建默认配置
        _config ??= new AppConfig();
        return _config;
    }
    
    /// <summary>
    /// 获取API基础地址
    /// </summary>
    /// <returns>API基础地址</returns>
    public static string GetApiBaseUrl()
    {
        return GetConfig().ApiBaseUrl;
    }

}
