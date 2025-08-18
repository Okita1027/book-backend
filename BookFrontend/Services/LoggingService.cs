using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace book_frontend.Services
{
    /// <summary>
    /// 统一的日志记录和异常处理服务
    /// </summary>
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 记录异常并显示用户友好的错误消息
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="userMessage">显示给用户的友好消息</param>
        /// <param name="context">异常发生的上下文信息</param>
        /// <param name="showDialog">是否显示错误对话框</param>
        public void LogErrorAndShowMessage(Exception exception, string userMessage, string context = "", bool showDialog = true)
        {
            // 记录详细的异常信息到日志
            _logger.LogError(exception, "异常发生在: {Context}. 用户消息: {UserMessage}", context, userMessage);

            // 显示用户友好的错误消息
            if (showDialog)
            {
                MessageBox.Show(userMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 记录警告信息并显示用户友好的警告消息
        /// </summary>
        /// <param name="message">警告消息</param>
        /// <param name="context">警告发生的上下文信息</param>
        /// <param name="showDialog">是否显示警告对话框</param>
        public void LogWarningAndShowMessage(string message, string context = "", bool showDialog = true)
        {
            // 记录警告信息到日志
            _logger.LogWarning("警告发生在: {Context}. 消息: {Message}", context, message);

            // 显示用户友好的警告消息
            if (showDialog)
            {
                MessageBox.Show(message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">信息消息</param>
        /// <param name="context">信息发生的上下文</param>
        public void LogInformation(string message, string context = "")
        {
            _logger.LogInformation("{Context}: {Message}", context, message);
        }

        /// <summary>
        /// 记录成功操作并显示成功消息
        /// </summary>
        /// <param name="message">成功消息</param>
        /// <param name="context">操作上下文</param>
        /// <param name="showDialog">是否显示成功对话框</param>
        public void LogSuccessAndShowMessage(string message, string context = "", bool showDialog = true)
        {
            // 记录成功信息到日志
            _logger.LogInformation("成功操作在: {Context}. 消息: {Message}", context, message);

            // 显示用户友好的成功消息
            if (showDialog)
            {
                MessageBox.Show(message, "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 记录API调用错误（无异常对象）
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="context">上下文信息</param>
        /// <param name="operation">操作类型</param>
        /// <param name="showDialog">是否显示对话框</param>
        public void LogApiError(string errorMessage, string context, string operation, bool showDialog = true)
        {
            _logger.LogError("API调用失败 - 上下文: {Context}, 操作: {Operation}, 错误: {ErrorMessage}", 
                context, operation, errorMessage);

            if (showDialog)
            {
                MessageBox.Show($"操作失败：{errorMessage}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 记录API调用异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="apiEndpoint">API端点</param>
        /// <param name="operation">操作类型</param>
        /// <param name="userMessage">用户友好消息</param>
        public void LogApiError(Exception exception, string apiEndpoint, string operation, string userMessage)
        {
            _logger.LogError(exception, "API调用失败 - 端点: {ApiEndpoint}, 操作: {Operation}, 异常: {Exception}", 
                apiEndpoint, operation, exception.Message);

            MessageBox.Show(userMessage, "网络错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 记录验证错误
        /// </summary>
        /// <param name="validationErrors">验证错误信息</param>
        /// <param name="context">验证上下文</param>
        public void LogValidationErrors(string validationErrors, string context = "")
        {
            _logger.LogWarning("验证失败在: {Context}. 错误: {ValidationErrors}", context, validationErrors);
        }

        /// <summary>
        /// 记录警告消息并显示对话框
        /// </summary>
        /// <param name="message">警告消息</param>
        /// <param name="title">对话框标题</param>
        /// <param name="showDialog">是否显示对话框</param>
        public void LogWarningMessage(string message, string title = "警告", bool showDialog = true)
        {
            _logger.LogWarning("警告消息: {Message}", message);

            if (showDialog)
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 记录成功消息并显示对话框
        /// </summary>
        /// <param name="message">成功消息</param>
        /// <param name="title">对话框标题</param>
        /// <param name="showDialog">是否显示对话框</param>
        public void LogSuccessMessage(string message, string title = "成功", bool showDialog = true)
        {
            _logger.LogInformation("成功消息: {Message}", message);

            if (showDialog)
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}