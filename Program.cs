using System.Text;
using DEMO_CRUD.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// 配置日志
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// JWT配置开始
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"];    // JWT签名密钥
string issuer = jwtSettings["Issuer"];  // JWT令牌发布者
string audience = jwtSettings["Audience"];  // JWT令牌接收者
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT 密钥没有配置好");
}

byte[] key = Encoding.ASCII.GetBytes(secretKey);    // 将密钥字符串转为字节数组
builder.Services.AddAuthentication(options =>
{
    // 设置默认的认证方案和挑战方案为JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // 在开发环境可以设置为false,生产环境必须设置为true，强制HTTPS
    options.RequireHttpsMetadata = false;
    options.SaveToken = true; // 是否在HttpContext中存储JWT
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true, // 验证签名密钥，确保令牌没有被篡改
        IssuerSigningKey = new SymmetricSecurityKey(key),   // 用于签名的密钥
        ValidateIssuer = true, // 验证令牌的发布者
        ValidateAudience = true, // 验证令牌的接收者
        ValidateLifetime = true, // 验证令牌有效期（过期时间）
        ValidIssuer = issuer, // 有效的发布者
        ValidAudience = audience, // 有效的接收者
        ClockSkew = TimeSpan.Zero // 令牌有效期时钟偏移量，默认为5分钟，这里设置为0，表示令牌有效期必须与当前时间一致
    };
    
});
builder.Services.AddAuthentication();   // 启用授权服务
// JWT配置结束

// 从配置文件中获取连接字符串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 注册DbContext服务
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 告诉 DbContext 使用 MySQL 数据库
    // 使用 AutoDetect 可以让 Pomelo 自动侦测MySQL版本并套用最佳设定
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 处理循环依赖的一种简单方式，但不推荐这么做，这会让前端难以处理返回的数据
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // 处理循环引用
//    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();  // 第三方日志中间件-Serilog


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 认证中间件必须在授权中间件之前
app.UseAuthentication(); // 启用认证，解析并验证请求中的 JWT
app.UseAuthorization();  // 启用授权，根据用户身份和策略决定是否允许访问

app.MapControllers();

app.Run();
