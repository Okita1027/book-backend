namespace book_backend.utils;

/// <summary>
/// 统一API响应结果封装类
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class Result<T>
{
    /// <summary>
    /// 响应码
    /// </summary>
    public int Code { get; set; }
    
    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// 响应数据
    /// </summary>
    public T Data { get; set; }
    
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public Result()
    {
    }
    
    public Result(int code, string message, T data)
    {
        Code = code;
        Message = message;
        Data = data;
    }
}

/// <summary>
/// 无数据的统一API响应结果封装类
/// </summary>
public class Result : Result<object>
{
    public Result()
    {
    }
    
    public Result(int code, string message, object data) : base(code, message, data)
    {
    }
}
