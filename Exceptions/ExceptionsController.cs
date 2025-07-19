using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DEMO_CRUD.Exceptions;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)] //指示Swagger忽略此控制器
public class ExceptionsController(ILogger<ExceptionsController> logger, IHostEnvironment environment) : ControllerBase
{
    public IActionResult Exception()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        if (exception != null && environment.IsDevelopment())
        {
            string originalPath = feature?.Path ?? HttpContext.Request.Path;
            string user = User.Identity?.Name ?? "anonymous";
            string remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            logger.LogError(
                exception,
                "发生未处理的异常，请求路径：{OriginalPath}，用户：{User}，IP：{RemoteIP}",
                originalPath,
                user,
                remoteIp);
        }

        if (environment.IsProduction())
        {
            return Problem(
                title: "服务器内部错误",
                statusCode: 500);
        }

        return Problem(
            detail: exception?.Message,
            statusCode: 500,
            title: "服务器内部错误"
        );
    }
}