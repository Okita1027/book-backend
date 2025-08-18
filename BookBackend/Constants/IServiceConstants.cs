namespace book_backend.Constants;

public interface IServiceConstants
{
    const string OPERATION_SUCCESS = "操作成功";
    const string OPERATION_FAILED = "操作失败";

    const string RECORD_NOT_FOUND = "相关记录不存在";
    const string RESOURCE_NOT_FOUND = "相关资源不存在";
    const string INVAILD_INPUT = "相关资源不存在";
    
    const string BOOK_NOT_FOUND = "书籍不存在";
    const string ONE_CATEGORY_NEED = "至少需要一个分类";
    const string ISBN_ALREADY_EXISTS = "ISBN已存在";
    const string PUBLISHER_ALREADY_EXISTS = "出版社已存在";
    const string CATEGORY_ALREADY_EXISTS = "分类已存在";
    const string AUTHOR_NOT_FOUND = "作者不存在";
    const string PUBLISHER_NOT_FOUND = "出版社不存在";
    const string AVAILABLE_EXCEEDS_STOCK = "可用库存不能大于总库存";
    const string NOT_ENOUGH_AVAILABLE = "无可用资源";
    const string CATEGORY_NOT_FOUND = "书籍类别不存在";
    const string USER_NOT_FOUND = "用户不存在";
    const string EMAIL_PASSWORD_INCORRECT = "邮箱或密码错误";
    const string INCORRECT_FIELD_VALIDATION = "数据格式错误";
    const string EMAIL_ALREADY_REGISTERED = "邮箱已被注册";
}