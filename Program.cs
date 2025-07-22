using System.Reflection;
using System.Text;
using DEMO_CRUD.Data;
using DEMO_CRUD.Exceptions;
using DEMO_CRUD.Services;
using DEMO_CRUD.Services.Impl;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// 配置日志
// var logPath = builder.Configuration["Logging:LogPath"] ?? "Logs/";
// var logFilePath = Path.Combine(logPath, "log-.txt");
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Destructure.ByTransforming<DateTime>(datetime => datetime.ToLocalTime())
    // .MinimumLevel.Information()
    // .WriteTo.File(
    //     new RenderedCompactJsonFormatter(), // 日志格式化为JSON
    //     logFilePath,
    //     rollingInterval: RollingInterval.Day,
    //     retainedFileCountLimit: 7)  // 保留最近7天的日志
    .CreateLogger();


// Add services to the container.
builder.Services.AddScoped<IBooksService, BooksServiceImpl>();
builder.Services.AddScoped<IUsersService, UsersServiceImpl>();
builder.Services.AddScoped<ILoansService, LoansServiceImpl>();
// JWT配置开始
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"]; // JWT签名密钥
string issuer = jwtSettings["Issuer"]; // JWT令牌发布者
string audience = jwtSettings["Audience"]; // JWT令牌接收者
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT 密钥没有配置好");
}

byte[] key = Encoding.ASCII.GetBytes(secretKey); // 将密钥字符串转为字节数组
builder.Services.AddAuthentication(options =>
{
    // 设置默认的认证方案和挑战方案为JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // 是否启用强制HTTPS
    options.RequireHttpsMetadata = false;
    // 是否在HttpContext中存储JWT
    options.SaveToken = true; 
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true, // 验证签名密钥，确保令牌没有被篡改
        IssuerSigningKey = new SymmetricSecurityKey(key), // 用于签名的密钥
        ValidateIssuer = true, // 验证令牌的发布者
        ValidateAudience = true, // 验证令牌的接收者
        ValidateLifetime = true, // 验证令牌有效期（过期时间）
        ValidIssuer = issuer, // 有效的发布者
        ValidAudience = audience, // 有效的接收者
        ClockSkew = TimeSpan.Zero // 令牌有效期时钟偏移量，默认为5分钟，这里设置为0，表示令牌有效期必须与当前时间一致
    };
});
builder.Services.AddAuthentication(); // 启用授权服务
// JWT配置结束

// 从配置文件中获取连接字符串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 注册DbContext服务
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // AutoDetect 可以让 Pomelo 自动侦测MySQL版本并套用最佳设定
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 处理循环引用的一种简单方式，但不推荐这么做，这会让前端难以处理返回的数据
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    });

// 注册Mapster核心服务
builder.Services.AddMapster();
// 执行自定义的Mapster映射配置
MapsterConfig.Configure();
builder.Services.AddResponseCaching();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ArgumentExceptionFilter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 第三方日志中间件-Serilog
builder.Host.UseSerilog();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseExceptionHandler("/exceptions");
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/exceptions");
}
// app.UseRouting();    // .NET 6之后，若使用了MapXXX，则自动启用UseRouting()

// 认证中间件必须在授权中间件之前
app.UseAuthentication(); // 启用认证，解析并验证请求中的 JWT
app.UseAuthorization(); // 启用授权，根据用户身份和策略决定是否允许访问

app.UseResponseCaching();

// 记录每次请求的详细信息，包括路径、状态码、耗时等。
app.UseSerilogRequestLogging();

app.MapControllers();

Log.Information("Application is starting...");

app.Run();

Log.Information("Application is shutting down...");
Log.CloseAndFlush();