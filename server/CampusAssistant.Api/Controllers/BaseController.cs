using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CampusAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    protected string UserRole => User.FindFirstValue(ClaimTypes.Role)!;
    protected string UserName => User.FindFirstValue(ClaimTypes.Name) ?? "";
}

[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;
    public AuthController(AppDbContext db, IConfiguration cfg) { _db = db; _cfg = cfg; }

    // 登录：校验密码 + 签发 JWT + 记录登录日志
    [HttpPost("login"), AllowAnonymous]
    public async Task<ApiResult> Login(LoginReq req)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        {
            _db.LoginLogs.Add(new LoginLog { Username = req.Username ?? "", Ip = ip, Result = "fail", Message = "参数缺失" });
            await _db.SaveChangesAsync();
            return ApiResult.Fail("用户名和密码不能为空", 400);
        }
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
        {
            _db.LoginLogs.Add(new LoginLog { Username = req.Username, Ip = ip, Result = "fail", Message = "用户名或密码错误" });
            await _db.SaveChangesAsync();
            return ApiResult.Fail("用户名或密码错误", 401);
        }
        if (user.Status == "disabled")
        {
            _db.LoginLogs.Add(new LoginLog { UserId = user.Id, Username = user.Username, Ip = ip, Result = "fail", Message = "账号已被禁用" });
            await _db.SaveChangesAsync();
            return ApiResult.Fail("账号已被禁用，请联系管理员", 403);
        }

        var token = GenerateToken(user);
        _db.LoginLogs.Add(new LoginLog { UserId = user.Id, Username = user.Username, Ip = ip, Result = "success", Message = "登录成功" });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { token, user = new { user.Id, user.Username, user.Name, user.Role, user.StudentNo, user.Phone } });
    }

    // 注册：表单校验 + BCrypt 加密 + 用户名重复检查（仅学生可自助注册）
    [HttpPost("register"), AllowAnonymous]
    public async Task<ApiResult> Register(RegisterReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || req.Username.Length < 3 || req.Username.Length > 20)
            return ApiResult.Fail("用户名长度需为 3-20 位");
        if (req.Username.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            return ApiResult.Fail("用户名只能包含字母、数字和下划线");
        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
            return ApiResult.Fail("密码长度不能少于 6 位");
        if (string.IsNullOrWhiteSpace(req.Name))
            return ApiResult.Fail("姓名不能为空");
        if (!string.IsNullOrWhiteSpace(req.Phone) && !System.Text.RegularExpressions.Regex.IsMatch(req.Phone, @"^1[3-9]\d{9}$"))
            return ApiResult.Fail("手机号格式不正确");
        if (await _db.SysUsers.AnyAsync(u => u.Username == req.Username))
            return ApiResult.Fail("用户名已存在，请更换");

        var u = new SysUser
        {
            Username = req.Username,
            Name = req.Name,
            Role = "student",
            StudentNo = req.StudentNo,
            Phone = req.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };
        _db.SysUsers.Add(u);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { u.Id, message = "注册成功，请登录" });
    }

    [HttpGet("profile"), Authorize]
    public async Task<ApiResult> Profile()
    {
        var u = await _db.SysUsers.FindAsync(UserId);
        return ApiResult.Ok(new { u!.Id, u.Username, u.Name, u.Role, u.StudentNo, u.Phone, u.Email, u.Status, u.CreateTime });
    }

    // 修改个人资料
    [HttpPost("profile"), Authorize]
    public async Task<ApiResult> UpdateProfile(UpdateProfileReq req)
    {
        var u = await _db.SysUsers.FindAsync(UserId);
        if (u == null) return ApiResult.Fail("用户不存在", 404);
        if (string.IsNullOrWhiteSpace(req.Name)) return ApiResult.Fail("姓名不能为空");
        if (!string.IsNullOrWhiteSpace(req.Phone) && !System.Text.RegularExpressions.Regex.IsMatch(req.Phone, @"^1[3-9]\d{9}$"))
            return ApiResult.Fail("手机号格式不正确");
        u.Name = req.Name; u.Phone = req.Phone; u.Email = req.Email; u.UpdateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { message = "资料已更新" });
    }

    // 修改密码：校验原密码 + 新密码长度校验
    [HttpPost("change-password"), Authorize]
    public async Task<ApiResult> ChangePassword(ChangePwdReq req)
    {
        if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 6)
            return ApiResult.Fail("新密码长度不能少于 6 位");
        var u = await _db.SysUsers.FindAsync(UserId);
        if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, u!.Password))
            return ApiResult.Fail("原密码错误");
        u.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        u.UpdateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { message = "密码修改成功" });
    }

    private string GenerateToken(SysUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(
            _cfg["Jwt:Issuer"], _cfg["Jwt:Audience"], claims,
            expires: DateTime.Now.AddHours(double.Parse(_cfg["Jwt:ExpireHours"] ?? "12")),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

[Route("api/users"), Authorize(Roles = "admin")]
public class UserController : BaseController
{
    private readonly AppDbContext _db;
    public UserController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ApiResult> List([FromQuery] string? role, [FromQuery] string? keyword)
    {
        var q = _db.SysUsers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(role)) q = q.Where(u => u.Role == role);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(u => u.Name.Contains(keyword) || u.Username.Contains(keyword));
        var list = await q.OrderByDescending(u => u.Id)
            .Select(u => new { u.Id, u.Username, u.Name, u.Role, u.StudentNo, u.Phone, u.Status, u.CreateTime }).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 分页查询：返回总记录数 total 与当前页数据 list
    [HttpGet("page")]
    public async Task<ApiResult> Page([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? role = null, [FromQuery] string? keyword = null)
    {
        var q = _db.SysUsers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(role)) q = q.Where(u => u.Role == role);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(u => u.Name.Contains(keyword) || u.Username.Contains(keyword));
        var all = await q.OrderByDescending(u => u.Id)
            .Select(u => new { u.Id, u.Username, u.Name, u.Role, u.StudentNo, u.Phone, u.Status, u.CreateTime }).ToListAsync();
        return ApiResult.Ok(PagedResult<object>.Of(all, pageIndex, pageSize));
    }

    [HttpPost]
    public async Task<ApiResult> Create(CreateUserReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Username)) return ApiResult.Fail("用户名不能为空");
        if (await _db.SysUsers.AnyAsync(u => u.Username == req.Username))
            return ApiResult.Fail("用户名已存在");
        if (req.Role != "student" && req.Role != "teacher" && req.Role != "logistics" && req.Role != "admin")
            return ApiResult.Fail("角色不合法");
        var u = new SysUser
        {
            Username = req.Username, Name = req.Name, Role = req.Role,
            StudentNo = req.StudentNo, Phone = req.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword("123456")
        };
        _db.SysUsers.Add(u);
        await _db.SaveChangesAsync();
        _db.OperationLogs.Add(new OperationLog { UserId = UserId, Username = UserName, Module = "用户管理", Action = "新增用户", Detail = $"创建账号 {u.Username}（{u.Role}）", Ip = HttpContext.Connection.RemoteIpAddress?.ToString() });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { u.Id, message = "创建成功，初始密码 123456" });
    }

    // 启用 / 禁用账号
    [HttpPost("{id}/status")]
    public async Task<ApiResult> SetStatus(long id, StatusReq req)
    {
        var u = await _db.SysUsers.FindAsync(id);
        if (u == null) return ApiResult.Fail("用户不存在", 404);
        if (u.Role == "admin") return ApiResult.Fail("不能禁用管理员账号");
        u.Status = req.Status == "disabled" ? "disabled" : "enabled";
        u.UpdateTime = DateTime.Now;
        _db.OperationLogs.Add(new OperationLog { UserId = UserId, Username = UserName, Module = "用户管理", Action = u.Status == "disabled" ? "禁用账号" : "启用账号", Detail = $"{u.Username}", Ip = HttpContext.Connection.RemoteIpAddress?.ToString() });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { u.Status });
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ApiResult> ResetPassword(long id)
    {
        var u = await _db.SysUsers.FindAsync(id);
        if (u == null) return ApiResult.Fail("用户不存在", 404);
        u.Password = BCrypt.Net.BCrypt.HashPassword("123456");
        u.UpdateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { message = "密码已重置为 123456" });
    }

    [HttpDelete("{id}")]
    public async Task<ApiResult> Delete(long id)
    {
        var u = await _db.SysUsers.FindAsync(id);
        if (u == null) return ApiResult.Fail("用户不存在", 404);
        if (u.Role == "admin") return ApiResult.Fail("不能删除管理员");
        _db.SysUsers.Remove(u);
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}
