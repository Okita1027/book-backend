using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DEMO_CRUD.Exceptions;

public class ArgumentExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ArgumentException argumentException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Title = "参数异常——来自ArgumentExceptionFilter",
                Detail = argumentException.Message,
                StatusCode = 400
            });

            context.ExceptionHandled = true;
        }
    }
}