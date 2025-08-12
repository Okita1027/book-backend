using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using book_frontend.Models;

namespace book_frontend.Helpers;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _authToken;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
        // 处理后端返回的自定义日期格式，例如 "yyyy-MM-dd HH:mm:ss"
        _jsonOptions.Converters.Add(new JsonDateTimeConverter());
        _jsonOptions.Converters.Add(new NullableJsonDateTimeConverter());

        // 这些设置已在 DI 的 HttpClient 中配置，这里不再重复设置，避免重复添加头
        // _httpClient.BaseAddress = new Uri("http://localhost:8888/api/");
        // _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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
    /// GET请求
    /// </summary>
    /// <typeparam name="T">期望返回的数据类型</typeparam>
    /// <param name="endpoint">API端点路径，如"books"、"users/1"</param>
    /// <returns>包含响应数据和状态信息的ApiResponse对象</returns>
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            // 发送GET请求到指定端点
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            // 读取响应内容为字符串
            string content = await response.Content.ReadAsStringAsync();

            // 检查响应状态码是否表示成功（2XX）
            if (response.IsSuccessStatusCode)
            {
                // 反序列化响应内容为指定类型T的对象
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

    /// <summary>
    /// POST请求
    /// </summary>
    /// <typeparam name="T">期望返回的数据类型</typeparam>
    /// <param name="endpoint">API端点路径</param>
    /// <param name="data">要发送的数据对象，会被序列化为JSON</param>
    /// <returns>包含响应数据和状态信息的ApiResponse对象</returns>
    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            // 将请求数据序列化为JSON字符串，如果data为null，则生成空字符串
            string json = data != null ? JsonSerializer.Serialize(data, _jsonOptions) : "";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = result,
                    Code = (int)response.StatusCode,
                };
            }

            return new ApiResponse<T>
            {
                Success = false,
                Code = (int)response.StatusCode,
                Message = $"请求失败: {response.StatusCode}",
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

    /// <summary>
    /// PUT请求
    /// </summary>
    /// <typeparam name="T">期望返回的数据类型</typeparam>
    /// <param name="endpoint">API端点路径</param>
    /// <param name="data">要更新的数据对象</param>
    /// <returns>包含响应数据和状态信息的ApiResponse对象</returns>
    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = result,
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
                Success = false,
                Message = $"网络错误: {ex.Message}",
                Code = 500,
            };
        }
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
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrEmpty(str))
                return null;

            if (DateTime.TryParseExact(str, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out var dt))
                return dt;

            if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt2))
                return dt2;
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