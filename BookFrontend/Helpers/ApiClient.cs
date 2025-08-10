using System.Net.Http;
using System.Text;
using System.Text.Json;
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

        _httpClient.BaseAddress = new Uri("http://localhost:8888/api/");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    /// <param name="token">JWT Token字符串</param>
    public void SetAuthToken(string? token)
    {
        _authToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            // 如果Token不为空，将其添加到请求头的Authorization字段中
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            // 否则清除认证头信息
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
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