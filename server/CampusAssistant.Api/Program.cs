using System.Text;
using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using CampusAssistant.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 确保上传目录存在（wwwroot/uploads），供报修现场照片访问
var webRoot = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));

// 数据库：MySQL 8 + EF Core，表名/列名自动转蛇形（sys_user、create_time…）
var connStr = builder.Configuration.GetConnectionString("MySql")!;
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr))
       .UseSnakeCaseNamingConvention());

// JWT 认证
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    });
builder.Services.AddAuthorization();

// CORS：开发期放开，供两个前端联调
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHttpClient("ai", c => c.Timeout = TimeSpan.FromSeconds(15));
builder.Services.AddScoped<AiService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger 页面支持填入 Token 调试受保护接口
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "粘贴登录接口返回的 Token（不要带 Bearer 前缀）"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 全局异常中间件：统一错误 JSON
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(ApiResult.Fail("服务器内部错误：" + ex.Message, 500));
    }
});

// 启动时建库 + 种子数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    AppDbContext.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRoot),
    RequestPath = ""
});   // 提供 /uploads 静态图片访问
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 启动：把"端口被占用"这类致命错误转成清晰中文提示，避免抛出难懂的长堆栈
try
{
    app.Run();
}
catch (System.IO.IOException ex) when (
    ex.InnerException is Microsoft.AspNetCore.Connections.AddressInUseException
    || ex.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    Console.WriteLine("======================================================");
    Console.WriteLine("  后端启动失败：端口 5000 已被占用。");
    Console.WriteLine("  通常是上一次的后端进程（CampusAssistant.Api.exe）没有退出。");
    Console.WriteLine("  解决办法（任选其一）：");
    Console.WriteLine("   1) 任务管理器里结束所有 CampusAssistant.Api / dotnet 进程后重试；");
    Console.WriteLine("   2) PowerShell 执行：");
    Console.WriteLine("      Get-Process CampusAssistant.Api,dotnet -ErrorAction SilentlyContinue | Stop-Process -Force");
    Console.WriteLine("   3) 换端口启动：dotnet run --urls http://localhost:5001");
    Console.WriteLine("======================================================");
    Console.ResetColor();
    Environment.Exit(1);
}
