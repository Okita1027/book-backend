namespace book_frontend.Exceptions;

public class BusinessException : Exception
{
    public string ErrorCode { get; }
    
    public BusinessException(string message) : base(message)
    {
    }
    
    public BusinessException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}