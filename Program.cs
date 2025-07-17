using DEMO_CRUD.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

// 配置日志
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 从配置文件中获取连接字符串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 注册DbContext服务
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 告诉 DbContext 使用 MySQL 数据库
    // 使用 AutoDetect 可以让 Pomelo 自动侦测MySQL版本并套用最佳设定
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 处理循环依赖的一种简单方式
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

app.UseAuthorization();

app.MapControllers();

app.Run();
