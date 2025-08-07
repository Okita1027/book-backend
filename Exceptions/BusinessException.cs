namespace book_backend.Exceptions;

public class BusinessException : Exception
{
    public int ErrorCode { get; }
    public BusinessException() : base()
    {
    }

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BusinessException(int errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}