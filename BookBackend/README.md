# 图书管理系统后端 API

## 项目简介

这是一个基于 ASP.NET Core 8.0 开发的图书管理系统后端 API，提供完整的图书馆管理功能，包括图书管理、用户管理、借阅管理、罚金管理等核心功能。

## 技术栈

- **框架**: ASP.NET Core 8.0
- **数据库**: MySQL
- **ORM**: Entity Framework Core
- **认证**: JWT Bearer Token
- **日志**: Serilog
- **对象映射**: Mapster
- **API文档**: Swagger/OpenAPI
- **缓存**: Response Caching

## 主要功能模块

### 📚 图书管理 (Books)
- 图书的增删改查
- 图书库存管理
- 图书分类管理
- 支持按作者、出版社、分类查询

### 👥 用户管理 (Users)
- 用户注册与登录
- JWT 身份认证
- 用户信息管理

### 📖 借阅管理 (Loans)
- 图书借阅与归还
- 借阅历史记录
- 借阅状态跟踪

### 💰 罚金管理 (Fines)
- 逾期罚金计算
- 罚金缴纳记录

### 👨‍💼 作者管理 (Authors)
- 作者信息管理
- 作者与图书关联

### 🏢 出版社管理 (Publishers)
- 出版社信息管理
- 出版社与图书关联

### 🏷️ 分类管理 (Categories)
- 图书分类管理
- 多对多分类关系

## 项目结构

```
BookBackend/
├── Controllers/          # API 控制器
│   ├── AuthorsController.cs
│   ├── BooksController.cs
│   ├── CategoriesController.cs
│   ├── FinesController.cs
│   ├── LoansController.cs
│   ├── PublishersController.cs
│   └── UsersController.cs
├── Models/              # 数据模型
│   ├── DTO/            # 数据传输对象
│   ├── Entity/         # 实体模型
│   └── VO/             # 视图对象
├── Services/           # 业务逻辑服务
│   ├── Impl/          # 服务实现
│   ├── IBooksService.cs
│   ├── ILoansService.cs
│   └── IUsersService.cs
├── Data/               # 数据访问层
│   ├── ApplicationDbContext.cs
│   ├── AuditableEntity.cs
│   └── MapsterConfig.cs
├── Exceptions/         # 异常处理
├── Migrations/         # 数据库迁移
├── Constants/          # 常量定义
├── utils/              # 工具类
└── Properties/         # 项目配置
```

## 环境要求

- .NET 8.0 SDK
- MySQL 8.0+
- Visual Studio 2022 或 VS Code

## 快速开始

### 1. 克隆项目

```bash
git clone <repository-url>
cd BookManagement/BookBackend
```

### 2. 配置数据库连接

编辑 `appsettings.json` 文件，修改数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DbContext": "server=localhost;port=3306;database=LibraryDb;user=root;password=your_password"
  }
}
```

### 3. 安装依赖包

```bash
dotnet restore
```

### 4. 数据库迁移

```bash
# 创建数据库
dotnet ef database update
```

### 5. 运行项目

```bash
dotnet run
```

项目将在 `https://localhost:7000` 启动，Swagger 文档可在 `https://localhost:7000/swagger` 访问。

## 配置说明

### JWT 配置

在 `appsettings.json` 中配置 JWT 相关参数：

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "LibraryManagementAPI",
    "Audience": "LibraryManagementFrontend"
  }
}
```

### 日志配置

项目使用 Serilog 进行日志记录，日志文件保存在 `Logs/` 目录下：

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.json",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

### CORS 配置

支持跨域请求配置，可在 `appsettings.json` 中设置允许的源：

```json
{
  "Cors": {
    "AllowOrigins": ["http://localhost:3000", "https://yourdomain.com"]
  }
}
```

## API 文档

### 认证相关

- `POST /api/users/login` - 用户登录
- `POST /api/users/register` - 用户注册

### 图书管理

- `GET /api/books` - 获取所有图书
- `GET /api/books/{id}` - 根据ID获取图书
- `POST /api/books` - 创建新图书
- `PUT /api/books/{id}` - 更新图书信息
- `DELETE /api/books/{id}` - 删除图书

### 借阅管理

- `GET /api/loans` - 获取借阅记录
- `POST /api/loans` - 创建借阅记录
- `PUT /api/loans/{id}/return` - 归还图书

更多 API 详情请访问 Swagger 文档。

## 数据库设计

### 核心实体

- **Book**: 图书信息
- **User**: 用户信息
- **Author**: 作者信息
- **Publisher**: 出版社信息
- **Category**: 图书分类
- **Loan**: 借阅记录
- **Fine**: 罚金记录
- **BookCategory**: 图书分类关联表

### 关系设计

- Book ↔ Author (多对一)
- Book ↔ Publisher (多对一)
- Book ↔ Category (多对多)
- User ↔ Loan (一对多)
- Book ↔ Loan (一对多)
- Loan ↔ Fine (一对一)

## 开发指南

### 添加新的 API 端点

1. 在相应的 Controller 中添加新方法
2. 在 Service 接口中定义业务逻辑
3. 在 Service 实现类中编写具体逻辑
4. 如需要，添加相应的 DTO 和 VO 类

### 数据库迁移

```bash
# 添加新迁移
dotnet ef migrations add MigrationName

# 更新数据库
dotnet ef database update

# 回滚迁移
dotnet ef database update PreviousMigrationName
```

### 测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试
dotnet test --filter "TestName"
```

## 部署

### Docker 部署

```dockerfile
# Dockerfile 示例
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["book-backend.csproj", "."]
RUN dotnet restore "book-backend.csproj"
COPY . .
RUN dotnet build "book-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "book-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "book_backend.dll"]
```

### 生产环境配置

1. 修改 `appsettings.Production.json`
2. 配置安全的数据库连接字符串
3. 设置强密码的 JWT 密钥
4. 配置 HTTPS 证书
5. 设置适当的日志级别

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 联系方式

如有问题或建议，请通过以下方式联系：

- 项目 Issues: [GitHub Issues](https://github.com/your-repo/issues)
- 邮箱: your-email@example.com

## 更新日志

### v1.0.0 (2024-01-XX)
- 初始版本发布
- 实现基础的图书管理功能
- 添加用户认证和授权
- 实现借阅管理系统
- 添加罚金管理功能

---

**注意**: 请确保在生产环境中更改默认的 JWT 密钥和数据库密码！