using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusAssistant.Api.Controllers;

// ============ 公告：浏览 / 管理 / 审核 ============

[Route("api/announcements"), Authorize]
public class AnnouncementController : BaseController
{
    private readonly AppDbContext _db;
    public AnnouncementController(AppDbContext db) { _db = db; }

    // 学生/教师：浏览已发布公告（分页 + 分类 + 关键词检索）
    [HttpGet]
    public async Task<ApiResult> List([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null, [FromQuery] string? keyword = null)
    {
        var q = _db.Announcements.Where(a => a.Status == "published");
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(a => a.Category == category);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(a => a.Title.Contains(keyword) || a.Content!.Contains(keyword));
        var all = await q.OrderByDescending(a => a.IsTop).ThenByDescending(a => a.CreateTime)
            .Select(a => new { a.Id, a.Title, a.Content, a.Category, a.Scope, a.IsTop, a.PublisherName, a.ViewCount, a.CreateTime }).ToListAsync();
        return ApiResult.Ok(PagedResult<object>.Of(all, pageIndex, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ApiResult> Detail(long id)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return ApiResult.Fail("公告不存在", 404);
        a.ViewCount++;
        await _db.SaveChangesAsync();
        return ApiResult.Ok(a);
    }

    // 教师/管理员：我发布 / 全部公告管理
    [HttpGet("manage"), Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Manage([FromQuery] string? status = null)
    {
        var q = _db.Announcements.AsQueryable();
        if (UserRole == "teacher") q = q.Where(a => a.PublisherId == UserId);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(a => a.Status == status);
        var list = await q.OrderByDescending(a => a.CreateTime).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 待审核列表（管理员）
    [HttpGet("pending"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Pending()
    {
        var list = await _db.Announcements.Where(a => a.Status == "pending")
            .OrderByDescending(a => a.CreateTime).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 发布公告（教师/管理员）：教师发布 → 待审核；管理员发布 → 直接发布
    [HttpPost, Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Create(AnnouncementReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Title)) return ApiResult.Fail("公告标题不能为空");
        if (string.IsNullOrWhiteSpace(req.Content)) return ApiResult.Fail("公告内容不能为空");
        var a = new Announcement
        {
            Title = req.Title, Content = req.Content, Category = req.Category, Scope = req.Scope,
            IsTop = req.IsTop, PublisherId = UserId,
            PublisherName = (await _db.SysUsers.FindAsync(UserId))?.Name,
            Status = UserRole == "admin" ? "published" : "pending"
        };
        _db.Announcements.Add(a);
        _db.OperationLogs.Add(new OperationLog { UserId = UserId, Username = UserName, Module = "公告管理", Action = "发布公告", Detail = $"《{a.Title}》（{a.Status}）", Ip = HttpContext.Connection.RemoteIpAddress?.ToString() });
        await _db.SaveChangesAsync();

        // 发布成功的全校公告，向用户推送消息
        if (a.Status == "published")
            await NotifyAll(a.Title, a.Content ?? "", "announce");

        // 教师提交待审核，通知管理员
        if (a.Status == "pending")
        {
            var admins = await _db.SysUsers.Where(u => u.Role == "admin").Select(u => u.Id).ToListAsync();
            foreach (var ad in admins)
                _db.Notifications.Add(new Notification { UserId = ad, Title = "公告待审核", Content = $"{a.PublisherName} 提交了公告《{a.Title}》，等待审核。", Type = "announce" });
            await _db.SaveChangesAsync();
        }
        return ApiResult.Ok(new { a.Id, a.Status, message = a.Status == "pending" ? "已提交，等待管理员审核" : "发布成功" });
    }

    [HttpPut("{id}"), Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Update(long id, AnnouncementReq req)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return ApiResult.Fail("公告不存在", 404);
        if (UserRole == "teacher" && a.PublisherId != UserId) return ApiResult.Fail("无权修改他人公告", 403);
        a.Title = req.Title; a.Content = req.Content; a.Category = req.Category; a.Scope = req.Scope;
        a.IsTop = req.IsTop; a.UpdateTime = DateTime.Now;
        if (UserRole == "teacher") a.Status = "pending";
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpDelete("{id}"), Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Delete(long id)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return ApiResult.Fail("公告不存在", 404);
        if (UserRole == "teacher" && a.PublisherId != UserId) return ApiResult.Fail("无权删除他人公告", 403);
        _db.Announcements.Remove(a);
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    // 审核公告（管理员）
    [HttpPost("{id}/audit"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Audit(long id, AnnouncementAuditReq req)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return ApiResult.Fail("公告不存在", 404);
        a.Status = req.Approve ? "published" : "rejected";
        a.UpdateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        if (req.Approve) await NotifyAll(a.Title, a.Content ?? "", "announce");
        return ApiResult.Ok(new { a.Status });
    }

    private async Task NotifyAll(string title, string content, string type)
    {
        var ids = await _db.SysUsers.Where(u => u.Status == "enabled").Select(u => u.Id).ToListAsync();
        foreach (var uid in ids)
            _db.Notifications.Add(new Notification { UserId = uid, Title = "新公告：" + title, Content = content.Length > 80 ? content[..80] + "…" : content, Type = type });
        await _db.SaveChangesAsync();
    }
}

// ============ 食堂菜单 ============

[Route("api/cafeteria"), Authorize]
public class CafeteriaController : BaseController
{
    private readonly AppDbContext _db;
    public CafeteriaController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ApiResult> List([FromQuery] string? canteen = null, [FromQuery] string? mealType = null)
    {
        var q = _db.CafeteriaMenus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(canteen)) q = q.Where(m => m.Canteen == canteen);
        if (!string.IsNullOrWhiteSpace(mealType)) q = q.Where(m => m.MealType == mealType);
        var list = await q.OrderBy(m => m.Canteen).ThenBy(m => m.MealType).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<ApiResult> Create(CafeteriaReq req)
    {
        if (string.IsNullOrWhiteSpace(req.DishName)) return ApiResult.Fail("菜品名称不能为空");
        if (req.Price < 0) return ApiResult.Fail("价格不能为负");
        var m = new CafeteriaMenu { Canteen = req.Canteen, Window = req.Window, MealType = req.MealType, DishName = req.DishName, Price = req.Price, Description = req.Description };
        _db.CafeteriaMenus.Add(m);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { m.Id });
    }

    [HttpPut("{id}"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Update(long id, CafeteriaReq req)
    {
        var m = await _db.CafeteriaMenus.FindAsync(id);
        if (m == null) return ApiResult.Fail("菜品不存在", 404);
        m.Canteen = req.Canteen; m.Window = req.Window; m.MealType = req.MealType;
        m.DishName = req.DishName; m.Price = req.Price; m.Description = req.Description;
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpDelete("{id}"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Delete(long id)
    {
        var m = await _db.CafeteriaMenus.FindAsync(id);
        if (m == null) return ApiResult.Fail("菜品不存在", 404);
        _db.CafeteriaMenus.Remove(m);
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

// ============ 消息中心 ============

[Route("api/notifications"), Authorize]
public class NotificationController : BaseController
{
    private readonly AppDbContext _db;
    public NotificationController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ApiResult> List([FromQuery] string? type = null)
    {
        var q = _db.Notifications.Where(n => n.UserId == UserId);
        if (!string.IsNullOrWhiteSpace(type)) q = q.Where(n => n.Type == type);
        var list = await q.OrderByDescending(n => n.CreateTime).Take(100).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpGet("unread-count")]
    public async Task<ApiResult> UnreadCount() =>
        ApiResult.Ok(new { count = await _db.Notifications.CountAsync(n => n.UserId == UserId && !n.IsRead) });

    [HttpPost("{id}/read")]
    public async Task<ApiResult> Read(long id)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (n == null) return ApiResult.Fail("消息不存在", 404);
        n.IsRead = true;
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpPost("read-all")]
    public async Task<ApiResult> ReadAll()
    {
        var list = await _db.Notifications.Where(n => n.UserId == UserId && !n.IsRead).ToListAsync();
        list.ForEach(n => n.IsRead = true);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { updated = list.Count });
    }
}

// ============ 日志管理（管理员） ============

[Route("api/logs"), Authorize(Roles = "admin")]
public class LogController : BaseController
{
    private readonly AppDbContext _db;
    public LogController(AppDbContext db) { _db = db; }

    [HttpGet("login")]
    public async Task<ApiResult> LoginLogs([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 15, [FromQuery] string? result = null)
    {
        var q = _db.LoginLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(result)) q = q.Where(l => l.Result == result);
        var all = await q.OrderByDescending(l => l.CreateTime)
            .Select(l => new { l.Id, l.Username, l.Ip, l.Result, l.Message, l.CreateTime }).ToListAsync();
        return ApiResult.Ok(PagedResult<object>.Of(all, pageIndex, pageSize));
    }

    [HttpGet("operation")]
    public async Task<ApiResult> OperationLogs([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 15, [FromQuery] string? keyword = null)
    {
        var q = _db.OperationLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(l => l.Module!.Contains(keyword) || l.Action!.Contains(keyword));
        var all = await q.OrderByDescending(l => l.CreateTime)
            .Select(l => new { l.Id, l.Username, l.Module, l.Action, l.Detail, l.Ip, l.CreateTime }).ToListAsync();
        return ApiResult.Ok(PagedResult<object>.Of(all, pageIndex, pageSize));
    }
}

// ============ 学生咨询（AI 助手问题提交 + 教师回复） ============

[Route("api/consultations"), Authorize]
public class ConsultationController : BaseController
{
    private readonly AppDbContext _db;
    public ConsultationController(AppDbContext db) { _db = db; }

    // 学生提交咨询
    [HttpPost, Authorize(Roles = "student")]
    public async Task<ApiResult> Submit(ConsultationReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Question)) return ApiResult.Fail("问题内容不能为空");
        var c = new Consultation { UserId = UserId, Question = req.Question, Category = req.Category };
        _db.Consultations.Add(c);
        await _db.SaveChangesAsync();

        var teachers = await _db.SysUsers.Where(u => u.Role == "teacher").Select(u => u.Id).ToListAsync();
        foreach (var t in teachers)
            _db.Notifications.Add(new Notification { UserId = t, Title = "学生咨询待回复", Content = $"有新的学生咨询（{req.Category}）：{req.Question}", Type = "system" });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { c.Id, message = "已提交，教师会尽快回复" });
    }

    // 学生看自己的咨询
    [HttpGet("my"), Authorize(Roles = "student")]
    public async Task<ApiResult> My()
    {
        var list = await _db.Consultations.Where(c => c.UserId == UserId)
            .OrderByDescending(c => c.CreateTime).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 教师/管理员：查看全部咨询
    [HttpGet, Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> List([FromQuery] string? status = null)
    {
        var q = _db.Consultations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(c => c.Status == status);
        var list = await (from c in q
                          join u in _db.SysUsers on c.UserId equals u.Id
                          orderby c.CreateTime descending
                          select new { c.Id, c.Question, c.Category, c.Reply, c.Status, c.CreateTime, c.ReplyTime, StudentName = u.Name, StudentNo = u.StudentNo }).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 教师回复
    [HttpPost("{id}/reply"), Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Reply(long id, ConsultationReplyReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Reply)) return ApiResult.Fail("回复内容不能为空");
        var c = await _db.Consultations.FindAsync(id);
        if (c == null) return ApiResult.Fail("咨询不存在", 404);
        c.Reply = req.Reply; c.ReplierId = UserId; c.Status = "replied"; c.ReplyTime = DateTime.Now;
        c.ReplierName = (await _db.SysUsers.FindAsync(UserId))?.Name;
        _db.Notifications.Add(new Notification { UserId = c.UserId, Title = "咨询已回复", Content = $"您的咨询已由 {c.ReplierName} 回复：{req.Reply}", Type = "system" });
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

// ============ 文件上传（报修现场照片等） ============

[Route("api/upload"), Authorize]
public class UploadController : BaseController
{
    private readonly IWebHostEnvironment _env;
    public UploadController(IWebHostEnvironment env) { _env = env; }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ApiResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return ApiResult.Fail("请选择要上传的文件");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allow = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        if (!allow.Contains(ext)) return ApiResult.Fail("仅支持 jpg/png/gif/webp 图片");
        if (file.Length > 5 * 1024 * 1024) return ApiResult.Fail("图片不能超过 5MB");

        var dir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(dir);
        var name = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(dir, name);
        using (var fs = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(fs);
        return ApiResult.Ok(new { url = $"/uploads/{name}" });
    }
}
