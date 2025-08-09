# 图书管理系统 WPF 前端

## 项目概述

这是一个基于 WPF (Windows Presentation Foundation) 技术栈开发的图书管理系统桌面客户端，采用 MVVM 架构模式，为图书馆管理提供完整的桌面解决方案。

## 系统功能

### 🔐 用户认证
- 用户登录/注册
- JWT Token 认证
- 角色权限管理（管理员/普通用户）

### 📚 图书管理
- 图书信息的增删改查
- 图书搜索
    - 按标题、ISBN、作者、出版社、分类等
    - 分页查询——无限滚动
- 库存管理

### 👥 作者管理
- 作者信息维护
- 作者传记管理
- 作者与图书关联

### 🏢 出版社管理
- 出版社信息管理
- 出版社与图书关联

### 📂 分类管理
- 图书分类体系
- 多分类支持

### 📖 借阅管理
- 图书借阅
- 图书归还
- 借阅记录查询
- 到期提醒

### 💰 罚款管理
- 逾期罚款计算
- 罚款记录管理
- 罚款缴纳处理

### 👤 用户管理
- 用户信息管理
- 用户借阅历史
- 用户罚款记录

## 技术架构

### 前端技术栈
- **框架**: WPF (.NET 8.0)
- **架构模式**: MVVM (Model-View-ViewModel)
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **HTTP 客户端**: HttpClient
- **JSON 序列化**: System.Text.Json
- **UI 框架**: Material Design In XAML Toolkit
- **异步编程**: async/await

### 后端 API
- **API 规范**: OpenAPI 3.0.1
- **基础 URL**: 待配置
- **认证方式**: JWT Bearer Token
- **数据格式**: JSON

## 项目结构

```
BookManagement.WPF/
├── Models/                 # 数据模型
│   ├── User.cs
│   ├── Book.cs
│   ├── Author.cs
│   ├── Publisher.cs
│   ├── Category.cs
│   ├── Loan.cs
│   └── Fine.cs
├── ViewModels/            # 视图模型
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── BookViewModel.cs
│   ├── AuthorViewModel.cs
│   ├── PublisherViewModel.cs
│   ├── CategoryViewModel.cs
│   ├── LoanViewModel.cs
│   └── UserViewModel.cs
├── Views/                 # 视图界面
│   ├── LoginWindow.xaml
│   ├── MainWindow.xaml
│   ├── BookManagementView.xaml
│   ├── AuthorManagementView.xaml
│   ├── PublisherManagementView.xaml
│   ├── CategoryManagementView.xaml
│   ├── LoanManagementView.xaml
│   └── UserManagementView.xaml
├── Services/              # 服务层
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IBookService.cs
│   ├── BookService.cs
│   ├── IAuthorService.cs
│   ├── AuthorService.cs
│   ├── IPublisherService.cs
│   ├── PublisherService.cs
│   ├── ICategoryService.cs
│   ├── CategoryService.cs
│   ├── ILoanService.cs
│   ├── LoanService.cs
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── INavigationService.cs
│   └── NavigationService.cs
├── Converters/            # 值转换器
│   ├── BooleanToVisibilityConverter.cs
│   ├── InverseBooleanConverter.cs
│   └── DateTimeConverter.cs
├── Commands/              # 命令实现
│   └── RelayCommand.cs
├── Helpers/               # 辅助类
│   ├── ApiClient.cs
│   └── ConfigurationHelper.cs
├── App.xaml              # 应用程序入口
├── App.xaml.cs
└── MainWindow.xaml       # 主窗口
```

## API 接口说明

### 认证接口
- `POST /api/Users/login` - 用户登录
- `POST /api/Users/register` - 用户注册

### 图书管理接口
- `GET /api/Books` - 获取图书列表
- `GET /api/Books/{id}` - 获取图书详情
- `POST /api/Books/add` - 添加图书
- `PUT /api/Books/{id}` - 更新图书
- `DELETE /api/Books/{id}` - 删除图书
- `GET /api/Books/searchPaginated` - 分页搜索图书

### 作者管理接口
- `GET /api/Authors` - 获取作者列表
- `GET /api/Authors/{id}` - 获取作者详情
- `POST /api/Authors` - 添加作者
- `PUT /api/Authors/{id}` - 更新作者
- `DELETE /api/Authors/{id}` - 删除作者
- `GET /api/Authors/{id}/books` - 获取作者的图书

### 出版社管理接口
- `GET /api/Publishers` - 获取出版社列表
- `GET /api/Publishers/{id}` - 获取出版社详情
- `POST /api/Publishers` - 添加出版社
- `PUT /api/Publishers/{id}` - 更新出版社
- `DELETE /api/Publishers/{id}` - 删除出版社

### 分类管理接口
- `GET /api/Categories` - 获取分类列表
- `GET /api/Categories/{id}` - 获取分类详情
- `POST /api/Categories` - 添加分类
- `PUT /api/Categories/{id}` - 更新分类
- `DELETE /api/Categories/{id}` - 删除分类

### 借阅管理接口
- `GET /api/Loans` - 获取借阅记录
- `GET /api/Loans/{id}` - 获取借阅详情
- `POST /api/Loan` - 创建借阅记录
- `PUT /api/Loans/{id}` - 更新借阅记录
- `DELETE /api/Loans/{id}` - 删除借阅记录
- `POST /api/Return` - 归还图书
- `GET /api/LoansByName` - 按用户名查询借阅记录

### 罚款管理接口
- `GET /api/Fines` - 获取罚款记录
- `GET /api/Fines/{id}` - 获取罚款详情
- `POST /api/Fines` - 创建罚款记录
- `PUT /api/Fines/{id}` - 更新罚款记录
- `DELETE /api/Fines/{id}` - 删除罚款记录
- `GET /api/FinesByName` - 按用户名查询罚款记录

### 用户管理接口
- `GET /api/Users` - 获取用户列表
- `GET /api/Users/{id}` - 获取用户详情
- `PUT /api/Users/{id}` - 更新用户信息
- `DELETE /api/Users/{id}` - 删除用户

## 数据模型

### 用户模型 (User)
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}

public enum UserRole
{
    User = 0,
    Admin = 1
}
```

### 图书模型 (Book)
```csharp
public class BookVO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public DateTime PublishedDate { get; set; }
    public int Stock { get; set; }
    public int Available { get; set; }
    public string AuthorName { get; set; }
    public string PublisherName { get; set; }
    public List<string> CategoryNames { get; set; }
}
```

### 借阅模型 (Loan)
```csharp
public class LoanVO
{
    public string Title { get; set; }
    public string Username { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
```

### 罚款模型 (Fine)
```csharp
public class FineVO
{
    public int LoadId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public double Amount { get; set; }
    public string Reason { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime CreatedTime { get; set; }
}
```

## 开发环境要求

- **操作系统**: Windows 10/11
- **.NET**: .NET 8.0 或更高版本
- **IDE**: Visual Studio 2022 或 JetBrains Rider
- **数据库**: 后端 API 支持的数据库

## 安装与运行

### 1. 克隆项目
```bash
git clone <repository-url>
cd book-frontend
```

### 2. 安装依赖
```bash
dotnet restore
```

### 3. 配置 API 地址
在 `appsettings.json` 中配置后端 API 地址：
```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-api-server.com"
  }
}
```

### 4. 运行应用
```bash
dotnet run
```

## 功能特性

### 🎨 用户界面
- Material Design 风格界面
- 响应式布局设计
- 深色/浅色主题切换
- 多语言支持准备

### 🔧 技术特性
- MVVM 架构模式
- 依赖注入容器
- 异步编程支持
- 数据绑定和命令绑定
- 值转换器支持
- 导航服务

### 📊 数据处理
- 分页数据加载
- 实时搜索过滤
- 数据验证
- 错误处理机制

### 🔒 安全特性
- JWT Token 认证
- 角色权限控制
- 安全的密码处理
- API 请求加密

## 开发指南

### MVVM 模式实践
1. **Model**: 数据模型，对应 API 返回的数据结构
2. **View**: XAML 界面，负责数据展示和用户交互
3. **ViewModel**: 业务逻辑，连接 View 和 Model

### 服务层设计
- 每个业务模块对应一个服务接口和实现
- 使用依赖注入管理服务生命周期
- 统一的错误处理和日志记录

### 数据绑定最佳实践
- 使用 `INotifyPropertyChanged` 实现属性变更通知
- 使用 `ObservableCollection` 管理集合数据
- 合理使用值转换器处理数据格式

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 联系方式

- 项目维护者: [Zhiyun Qin]
- 邮箱: [2368932388@qq.com]
- 项目链接: [https://github.com/Okita1027/book-frontend]

## 更新日志

### v1.0.0 (计划中)
- 🔄 基础 MVVM 架构搭建
- 🔄 用户认证功能
- 🔄 图书管理功能
- 🔄 借阅管理功能
- 🔄 Material Design UI
- 🔄 作者管理功能 (开发中)
- 🔄 出版社管理功能 (开发中)
- 🔄 分类管理功能 (开发中)
- 🔄 罚款管理功能 (开发中)
- 🔄 用户管理功能 (开发中)

---

**注意**: 这是一个 WPF 桌面应用程序，需要 Windows 操作系统运行。如需跨平台支持，建议考虑使用 Avalonia UI 或 MAUI 技术栈。