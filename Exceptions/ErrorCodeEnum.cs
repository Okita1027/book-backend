namespace DEMO_CRUD.Exceptions;

public enum ErrorCodeEnum
{
    Success = 200,
    
    NotFound = 404,
    InvalidInput = 400,
    Unauthorized = 401,
    Forbidden = 403,
    InternalServerError = 500,
}