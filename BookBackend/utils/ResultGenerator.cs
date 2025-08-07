namespace book_backend.utils;

/// <summary>
/// 统一结果生成工具类
/// </summary>
public class ResultGenerator
{

    public static Result<T> CreatedAtAction<T>(string message = "操作成功", T data = default(T))
    {
        return new Result<T>(201, message, data);
    }
    public static Result<T> NoContent<T>(string message = "操作成功", T data = default(T))
    {
        return new Result<T>(204, message, data);
    }
    
    /// <summary>
    /// 操作成功
    /// </summary>
    /// <param name="data">响应数据</param>
    /// <param name="message">响应消息</param>
    /// <returns>Result对象</returns>
    public static Result<T> Success<T>(T data, string message = "操作成功")
    {
        return new Result<T>(200, message, data);
    }
    
    /// <summary>
    /// 操作成功（无数据）
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>Result对象</returns>
    public static Result Success(string message = "操作成功")
    {
        return new Result(200, message, null);
    }

    public static Result<T> Failed<T>(int code, string message = "操作失败", T data = default(T))
    {
        return new Result<T>(code, message, data);
    }
    
    /// <summary>
    /// 操作失败
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="data">响应数据</param>
    /// <returns>Result对象</returns>
    public static Result<T> Error<T>(string message = "操作失败", T data = default(T))
    {
        return new Result<T>(500, message, data);
    }
    
    /// <summary>
    /// 操作失败（无数据）
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Result对象</returns>
    public static Result Error(string message = "操作失败")
    {
        return new Result(500, message, null);
    }
    
    /// <summary>
    /// 参数错误
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Result对象</returns>
    public static Result BadRequest(string message = "参数错误")
    {
        return new Result(400, message, null);
    }
    
    /// <summary>
    /// 未授权
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Result对象</returns>
    public static Result Unauthorized(string message = "未授权")
    {
        return new Result(401, message, null);
    }
    
    /// <summary>
    /// 禁止访问
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Result对象</returns>
    public static Result Forbidden(string message = "禁止访问")
    {
        return new Result(403, message, null);
    }
    
    /// <summary>
    /// 资源不存在
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Result对象</returns>
    public static Result NotFound(string message = "资源不存在")
    {
        return new Result(404, message, null);
    }
}
