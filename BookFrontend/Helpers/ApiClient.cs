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
            var response = await requestFunc();
            var content = await response.Content.ReadAsStringAsync();

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