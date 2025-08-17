using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using book_backend.Data;
using book_backend.Exceptions;
using book_backend.Services;
using book_backend.Services.Impl;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog.Events;
using Serilog.Formatting.Compact;

// 创建主机
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 配置日志
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Destructure.ByTransforming<DateTime>(datetime => datetime.ToLocalTime())
    .CreateLogger();

// 注册自定义服务
builder.Services.AddScoped<IBooksService, BooksServiceImpl>();
builder.Services.AddScoped<IUsersService, UsersServiceImpl>();
builder.Services.AddScoped<ILoansService, LoansServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

// 配置跨域
builder.Services.AddCors(options =>
{
    IConfigurationSection corsSettings = builder.Configuration.GetSection("Cors");
    string[] allowedOrigins = corsSettings.GetSection("AllowOrigins").Get<string[]>() ?? Array.Empty<string>();
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // 如果需要发送凭据（如 cookies）
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

// 日志增强配置
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .Destructure.ByTransforming<DateTime>(datetime => datetime.ToLocalTime()));


// JWT配置开始
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"]; // JWT签名密钥
string issuer = jwtSettings["Issuer"]; // JWT令牌发布者
string audience = jwtSettings["Audience"]; // JWT令牌接收者
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT 密钥没有配置好");
}

// 将密钥字符串转为字节数组
byte[] key = Encoding.ASCII.GetBytes(secretKey);
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
}, ServiceLifetime.Scoped);


// 注册Mapster核心服务、执行自定义的Mapster映射配置
builder.Services.AddMapster();
MapsterConfig.Configure();

builder.Services.AddResponseCaching();
builder.Services
    .AddControllers(options => { options.Filters.Add(new ArgumentExceptionFilter()); })
    .AddJsonOptions(options =>
    {
        // 配置JSON序列化时使用驼峰命名法（首字母小写）
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // 处理日期时间格式化
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
        // 处理循环引用的一种简单方式，但不推荐这么做，这会让前端难以处理返回的数据【此方案仅用于兜底】
        // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 第三方日志中间件-Serilog
builder.Host.UseSerilog();


WebApplication app = builder.Build();
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

// 使用名为AllowAll的跨域策略
app.UseCors("AllowAll");

// 认证中间件必须在授权中间件之前
app.UseAuthentication(); // 启用认证，解析并验证请求中的 JWT
app.UseAuthorization(); // 启用授权，根据用户身份和策略决定是否允许访问

app.UseResponseCaching();

// 记录每次请求的详细信息，包括路径、状态码、耗时等。
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null)
            return LogEventLevel.Error;
            
        if (httpContext.Response.StatusCode > 499)
            return LogEventLevel.Error;
            
        if (httpContext.Response.StatusCode > 399)
            return LogEventLevel.Warning;
            
        return LogEventLevel.Information;
    };
    
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
    };
});

app.MapControllers();

Log.Information("Application is starting...");

app.Run();

Log.Information("Application is shutting down...");
Log.CloseAndFlush();