using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Net;
using book_frontend.Models;
using book_frontend.Models.DTOs;

namespace book_frontend.Helpers;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _authToken;
    private string? _refreshToken;
    private DateTime _tokenExpiresAt;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    
    /// <summary>
    /// Token刷新事件，当Token被刷新时触发
    /// </summary>
    public event Action<string, string>? TokenRefreshed;
    
    /// <summary>
    /// 认证失败事件，当RefreshToken也过期时触发
    /// </summary>
    public event Action? AuthenticationFailed;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            // 将.NET对象的属性名转换为驼峰格式的JSON属性名
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // 忽略属性名的大小写
            PropertyNameCaseInsensitive = true,
        };
        // 处理后端返回的自定义日期格式，例如 "yyyy-MM-dd HH:mm:ss"
        _jsonOptions.Converters.Add(new JsonDateTimeConverter());
        _jsonOptions.Converters.Add(new NullableJsonDateTimeConverter());
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    public void SetAuthToken(string token)
    {
        _authToken = token;
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
    
    /// <summary>
    /// 设置认证Token和RefreshToken
    /// </summary>
    /// <param name="accessToken">访问Token</param>
    /// <param name="refreshToken">刷新Token</param>
    /// <param name="expiresAt">Token过期时间</param>
    public void SetTokens(string accessToken, string refreshToken, DateTime expiresAt)
    {
        _authToken = accessToken;
        _refreshToken = refreshToken;
        _tokenExpiresAt = expiresAt;
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    /// <summary>
    /// 清除所有Token
    /// </summary>
    public void ClearTokens()
    {
        _authToken = null;
        _refreshToken = null;
        _tokenExpiresAt = DateTime.MinValue;
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
    }
    
    /// <summary>
    /// 检查Token是否即将过期（提前5分钟刷新）
    /// </summary>
    /// <returns>是否需要刷新</returns>
    private bool IsTokenExpiringSoon()
    {
        return _tokenExpiresAt != DateTime.MinValue && 
               _tokenExpiresAt.Subtract(TimeSpan.FromMinutes(5)) <= DateTime.UtcNow;
    }
    
    /// <summary>
    /// 刷新访问Token
    /// </summary>
    /// <returns>是否刷新成功</returns>
    private async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken))
        {
            return false;
        }

        await _refreshSemaphore.WaitAsync();
        try
        {
            // 双重检查，避免并发刷新
            if (!IsTokenExpiringSoon())
            {
                return true;
            }

            var refreshRequest = new { RefreshToken = _refreshToken };
            var content = CreateJsonContent(refreshRequest);
            
            var response = await _httpClient.PostAsync("Auth/refresh-token", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, _jsonOptions);
                
                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    SetTokens(authResponse.Token, authResponse.RefreshToken, authResponse.ExpiresAt);
                    TokenRefreshed?.Invoke(authResponse.Token, authResponse.RefreshToken);
                    return true;
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // RefreshToken也过期了，触发认证失败事件
                ClearTokens();
                AuthenticationFailed?.Invoke();
            }
            
            return false;
        }
        catch (Exception ex)
        {
            // 刷新失败，记录日志但不抛出异常
            System.Diagnostics.Debug.WriteLine($"Token refresh failed: {ex.Message}");
            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    public Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        return SendRequestAsync<T>(() => _httpClient.GetAsync(endpoint));
    }

    public Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        var content = CreateJsonContent(data);
        return SendRequestAsync<T>(() => _httpClient.PostAsync(endpoint, content));
    }

    public Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        var content = CreateJsonContent(data);
        return SendRequestAsync<T>(() => _httpClient.PutAsync(endpoint, content));
    }

    /// <summary>
    /// DELETE请求
    /// </summary>
    /// <param name="endpoint">API端点路径</param>
    /// <returns>包含操作结果和状态信息的ApiResponse对象</returns>
    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

            return new ApiResponse<bool>
            {
                Success = response.IsSuccessStatusCode,
                Code = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? "删除成功"
                    : $"删除失败: {response.StatusCode}",
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Code = 500,
                Message = $"网络错误: {ex.Message}",
                Data = false,
            };
        }
    }

    private StringContent CreateJsonContent(object? data)
    {
        var json = data != null ? JsonSerializer.Serialize(data, _jsonOptions) : "";
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<ApiResponse<T>> SendRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc)
    {
        try
        {
            // 检查Token是否需要刷新（仅在有RefreshToken时）
            if (!string.IsNullOrEmpty(_refreshToken) && IsTokenExpiringSoon())
            {
                await RefreshTokenAsync();
            }

            var response = await requestFunc();
            var content = await response.Content.ReadAsStringAsync();

            // 如果返回401且有RefreshToken，尝试刷新Token后重试一次
            if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(_refreshToken))
            {
                var refreshSuccess = await RefreshTokenAsync();
                if (refreshSuccess)
                {
                    // Token刷新成功，重试原请求
                    response = await requestFunc();
                    content = await response.Content.ReadAsStringAsync();
                }
            }

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = data,
                    Code = (int)response.StatusCode,
                };
            }

            return new ApiResponse<T>
            {
                Success = false,
                Message = $"请求失败: {response.StatusCode}",
                Code = (int)response.StatusCode,
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Code = 500,
                Success = false,
                Message = $"网络错误: {ex.Message}",
            };
        }
    }
}

/// <summary>
/// System.Text.Json 的 DateTime 反序列化转换器，支持后端返回格式 "yyyy-MM-dd HH:mm:ss"
/// </summary>
public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrEmpty(str))
                return default;

            if (DateTime.TryParseExact(str, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out var dt))
                return dt;

            // 兜底，尝试常规解析
            if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt2))
                return dt2;
        }

        throw new JsonException($"无法将值转换为 DateTime。值: {reader.GetString()}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat));
    }
}

/// <summary>
/// 可空 DateTime 的转换器
/// </summary>
public class NullableJsonDateTimeConverter : JsonConverter<DateTime?>
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
            {
                var str = reader.GetString();
                if (string.IsNullOrEmpty(str))
                    return null;

                if (DateTime.TryParseExact(str, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out var dt))
                    return dt;

                if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt2))
                    return dt2;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new JsonException($"无法将值转换为 DateTime?。值: {reader.GetString()}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(DateTimeFormat));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}