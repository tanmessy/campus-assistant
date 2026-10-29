using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using CampusAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusAssistant.Api.Controllers;

// ============ 课程 / 成绩 / 预警 ============

[Route("api/courses"), Authorize]
public class CourseController : BaseController
{
    private readonly AppDbContext _db;
    public CourseController(AppDbContext db) { _db = db; }

    // 学生：我的课表；教师：我授的课；管理员：全部
    [HttpGet]
    public async Task<ApiResult> List()
    {
        if (UserRole == "student")
        {
            var list = await (from sc in _db.StudentCourses
                              join c in _db.Courses on sc.CourseId equals c.Id
                              join t in _db.SysUsers on c.TeacherId equals t.Id
                              where sc.StudentId == UserId
                              select new { c.Id, c.Name, c.Classroom, c.Schedule, c.Semester, c.Credit, TeacherName = t.Name }).ToListAsync();
            return ApiResult.Ok(list);
        }
        var q = _db.Courses.AsQueryable();
        if (UserRole == "teacher") q = q.Where(c => c.TeacherId == UserId);
        var all = await (from c in q
                         join t in _db.SysUsers on c.TeacherId equals t.Id
                         select new { c.Id, c.Name, c.Classroom, c.Schedule, c.Semester, c.Credit, c.TeacherId, TeacherName = t.Name }).ToListAsync();
        return ApiResult.Ok(all);
    }

    [HttpGet("{id}/students"), Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Students(long id)
    {
        var list = await (from sc in _db.StudentCourses
                          join u in _db.SysUsers on sc.StudentId equals u.Id
                          where sc.CourseId == id
                          select new
                          {
                              u.Id, u.Name, u.StudentNo,
                              Score = _db.Scores.Where(s => s.StudentId == u.Id && s.CourseId == id).Select(s => (decimal?)s.Value).FirstOrDefault(),
                              HasAlert = _db.AlertRecords.Any(a => a.StudentId == u.Id)
                          }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<ApiResult> Create(CourseReq req)
    {
        var c = new Course { Name = req.Name, TeacherId = req.TeacherId, Classroom = req.Classroom, Schedule = req.Schedule, Semester = req.Semester, Credit = req.Credit };
        _db.Courses.Add(c);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { c.Id });
    }

    [HttpPost("{id}/enroll"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Enroll(long id, [FromBody] long studentId)
    {
        if (await _db.StudentCourses.AnyAsync(x => x.CourseId == id && x.StudentId == studentId))
            return ApiResult.Fail("该学生已选此课");
        _db.StudentCourses.Add(new StudentCourse { CourseId = id, StudentId = studentId });
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

[Route("api/scores"), Authorize]
public class ScoreController : BaseController
{
    private readonly AppDbContext _db;
    public ScoreController(AppDbContext db) { _db = db; }

    // 学生查自己的成绩
    [HttpGet("my"), Authorize(Roles = "student")]
    public async Task<ApiResult> My()
    {
        var list = await (from s in _db.Scores
                          join c in _db.Courses on s.CourseId equals c.Id
                          where s.StudentId == UserId
                          select new { c.Name, c.Semester, c.Credit, s.Value }).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 教师/管理员录入成绩；<60 自动生成预警（alert_record）
    [HttpPost, Authorize(Roles = "teacher,admin")]
    public async Task<ApiResult> Entry(ScoreReq req)
    {
        var course = await _db.Courses.FindAsync(req.CourseId);
        if (course == null) return ApiResult.Fail("课程不存在", 404);

        var score = await _db.Scores.FirstOrDefaultAsync(s => s.StudentId == req.StudentId && s.CourseId == req.CourseId);
        if (score == null)
        {
            score = new Score { StudentId = req.StudentId, CourseId = req.CourseId, Value = req.Value, Semester = req.Semester };
            _db.Scores.Add(score);
        }
        else score.Value = req.Value;

        if (req.Value < 60 && !await _db.AlertRecords.AnyAsync(a => a.StudentId == req.StudentId && a.CourseName == course.Name))
        {
            _db.AlertRecords.Add(new AlertRecord
            {
                StudentId = req.StudentId, CourseName = course.Name,
                ScoreValue = req.Value, Reason = "成绩低于60分"
            });
        }
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { alertGenerated = req.Value < 60 });
    }
}

[Route("api/alerts"), Authorize]
public class AlertController : BaseController
{
    private readonly AppDbContext _db;
    public AlertController(AppDbContext db) { _db = db; }

    // 学生：我的预警；教师：所授课程的预警学生；管理员：全部
    [HttpGet, Authorize(Roles = "student,teacher,admin")]
    public async Task<ApiResult> List()
    {
        if (UserRole == "student")
        {
            var mine = await _db.AlertRecords.Where(a => a.StudentId == UserId)
                .OrderByDescending(a => a.CreateTime).ToListAsync();
            return ApiResult.Ok(mine);
        }
        if (UserRole == "teacher")
        {
            var courseIds = _db.Courses.Where(c => c.TeacherId == UserId).Select(c => c.Id);
            var list = await (from a in _db.AlertRecords
                              join u in _db.SysUsers on a.StudentId equals u.Id
                              join sc in _db.StudentCourses on a.StudentId equals sc.StudentId
                              where courseIds.Contains(sc.CourseId)
                              select new { a.Id, u.Name, u.StudentNo, a.CourseName, a.ScoreValue, a.Reason, a.CreateTime })
                             .Distinct().OrderByDescending(x => x.CreateTime).ToListAsync();
            return ApiResult.Ok(list);
        }
        var all = await (from a in _db.AlertRecords
                         join u in _db.SysUsers on a.StudentId equals u.Id
                         select new { a.Id, u.Name, u.StudentNo, a.CourseName, a.ScoreValue, a.Reason, a.CreateTime })
                        .OrderByDescending(x => x.CreateTime).ToListAsync();
        return ApiResult.Ok(all);
    }

    [HttpPost("{id}/read"), Authorize(Roles = "student")]
    public async Task<ApiResult> MarkRead(long id)
    {
        var a = await _db.AlertRecords.FirstOrDefaultAsync(x => x.Id == id && x.StudentId == UserId);
        if (a == null) return ApiResult.Fail("记录不存在", 404);
        a.IsRead = true;
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

// ============ 宿舍报修（含 AI 智能分类） ============

[Route("api/repairs"), Authorize]
public class RepairController : BaseController
{
    private readonly AppDbContext _db;
    private readonly AiService _ai;
    public RepairController(AppDbContext db, AiService ai) { _db = db; _ai = ai; }

    // 学生提交报修：AI 自动分类 + 预判优先级 + 生成维修建议 + 写入首条进度
    [HttpPost, Authorize(Roles = "student")]
    public async Task<ApiResult> Submit(RepairReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Description)) return ApiResult.Fail("故障描述不能为空");
        if (string.IsNullOrWhiteSpace(req.Dormitory)) return ApiResult.Fail("请填写报修地址");
        var (cat, pri, advice) = await _ai.ClassifyRepairAsync(req.Description ?? "");
        var order = new RepairOrder
        {
            UserId = UserId,
            Category = string.IsNullOrWhiteSpace(req.Category) ? cat : req.Category,
            Dormitory = req.Dormitory,
            Description = req.Description,
            Images = req.Images,
            Contact = req.Contact,
            Priority = pri,
            AiAdvice = advice
        };
        _db.RepairOrders.Add(order);
        await _db.SaveChangesAsync();

        _db.RepairProgresses.Add(new RepairProgress { RepairId = order.Id, Status = "pending", Remark = "学生提交报修，等待后勤接单", OperatorId = UserId, OperatorName = UserName ?? "学生" });
        // 通知后勤人员
        var logisticsIds = await _db.SysUsers.Where(u => u.Role == "logistics").Select(u => u.Id).ToListAsync();
        foreach (var lid in logisticsIds)
            _db.Notifications.Add(new Notification { UserId = lid, Title = "新报修工单", Content = $"{req.Dormitory} 提交了新的报修工单（{order.Category}类，紧急度：{pri}），请及时处理。", Type = "repair" });
        await _db.SaveChangesAsync();

        return ApiResult.Ok(new { order.Id, order.Category, order.Priority, order.AiAdvice, aiEnabled = _ai.HasKey });
    }

    // 列表：学生看自己的，后勤/管理员看全部（支持状态筛选 + 关键词）
    [HttpGet]
    public async Task<ApiResult> List([FromQuery] string? status, [FromQuery] string? keyword)
    {
        var q = _db.RepairOrders.AsQueryable();
        if (UserRole == "student") q = q.Where(r => r.UserId == UserId);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(r => r.Description!.Contains(keyword) || r.Dormitory!.Contains(keyword));
        var list = await (from r in q
                          join u in _db.SysUsers on r.UserId equals u.Id
                          orderby r.CreateTime descending
                          select new { r.Id, r.Category, r.Dormitory, r.Description, r.Images, r.Status, r.Priority, r.AiAdvice, r.Contact, r.Rating, r.Comment, r.CreateTime, r.UpdateTime, StudentName = u.Name }).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 分页列表
    [HttpGet("page")]
    public async Task<ApiResult> Page([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null, [FromQuery] string? keyword = null)
    {
        var q = _db.RepairOrders.AsQueryable();
        if (UserRole == "student") q = q.Where(r => r.UserId == UserId);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(r => r.Description!.Contains(keyword) || r.Dormitory!.Contains(keyword));
        var all = await (from r in q
                         join u in _db.SysUsers on r.UserId equals u.Id
                         orderby r.CreateTime descending
                         select new { r.Id, r.Category, r.Dormitory, r.Description, r.Images, r.Status, r.Priority, r.AiAdvice, r.Rating, r.CreateTime, StudentName = u.Name }).ToListAsync();
        return ApiResult.Ok(PagedResult<object>.Of(all, pageIndex, pageSize));
    }

    // 后勤人员列表（供分配工单使用，仅后勤/管理员可见）
    [HttpGet("handlers"), Authorize(Roles = "logistics,admin")]
    public async Task<ApiResult> Handlers()
    {
        var list = await _db.SysUsers.Where(u => u.Role == "logistics" && u.Status == "enabled")
            .Select(u => new { u.Id, u.Name, u.Username }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ApiResult> Detail(long id)
    {
        var r = await _db.RepairOrders.FindAsync(id);
        if (r == null) return ApiResult.Fail("工单不存在", 404);
        if (UserRole == "student" && r.UserId != UserId) return ApiResult.Fail("无权查看该工单", 403);
        var student = await _db.SysUsers.FindAsync(r.UserId);
        var handler = r.HandlerId == null ? null : await _db.SysUsers.FindAsync(r.HandlerId);
        var progress = await _db.RepairProgresses.Where(p => p.RepairId == id).OrderBy(p => p.CreateTime).ToListAsync();
        return ApiResult.Ok(new
        {
            r.Id, r.Category, r.Dormitory, r.Description, r.Images, r.Status, r.Priority, r.AiAdvice, r.Contact, r.Rating, r.Comment, r.CreateTime, r.UpdateTime,
            StudentName = student?.Name, HandlerName = handler?.Name, Progress = progress
        });
    }

    // 后勤/管理员处理工单：accept 接单 / assign 分配 / done 完工 / reject 驳回
    [HttpPost("{id}/handle"), Authorize(Roles = "logistics,admin")]
    public async Task<ApiResult> Handle(long id, RepairHandleReq req)
    {
        var order = await _db.RepairOrders.FindAsync(id);
        if (order == null) return ApiResult.Fail("工单不存在", 404);
        var opName = UserName ?? "后勤";

        switch (req.Action)
        {
            case "accept":
                if (order.Status != "pending") return ApiResult.Fail("该工单已被处理");
                order.Status = "processing"; order.HandlerId = UserId;
                break;
            case "assign":
                if (req.AssignTo == null) return ApiResult.Fail("请选择要分配的后勤人员");
                var target = await _db.SysUsers.FindAsync(req.AssignTo);
                if (target == null || target.Role != "logistics") return ApiResult.Fail("指派人员不存在或角色不符");
                order.HandlerId = req.AssignTo; order.Status = "processing";
                _db.Notifications.Add(new Notification { UserId = req.AssignTo.Value, Title = "工单分配提醒", Content = $"工单 #{id}（{order.Dormitory}）已分配给您，请及时处理。", Type = "repair" });
                opName = target.Name;
                break;
            case "done":
                order.Status = "done";
                break;
            case "reject":
                order.Status = "rejected";
                break;
            default:
                return ApiResult.Fail("不支持的操作");
        }
        order.UpdateTime = DateTime.Now;

        _db.RepairProgresses.Add(new RepairProgress { RepairId = id, Status = order.Status, Remark = req.Remark, Images = req.Images, OperatorId = UserId, OperatorName = opName });
        var statusText = order.Status switch { "processing" => "已接单，维修中", "done" => "已完工", "rejected" => "已驳回", _ => "处理中" };
        _db.Notifications.Add(new Notification { UserId = order.UserId, Title = "报修进度更新", Content = $"您的报修工单 #{id}（{order.Dormitory}）{statusText}。", Type = "repair" });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { order.Status });
    }

    // 后勤现场更新进度（上传现场照片）
    [HttpPost("{id}/progress"), Authorize(Roles = "logistics,admin")]
    public async Task<ApiResult> AddProgress(long id, RepairProgressReq req)
    {
        var order = await _db.RepairOrders.FindAsync(id);
        if (order == null) return ApiResult.Fail("工单不存在", 404);
        var opName = UserName ?? "后勤";
        _db.RepairProgresses.Add(new RepairProgress { RepairId = id, Status = req.Status, Remark = req.Remark, Images = req.Images, OperatorId = UserId, OperatorName = opName });
        order.Status = req.Status;
        order.UpdateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    // 学生查看工单进度时间轴
    [HttpGet("{id}/progress")]
    public async Task<ApiResult> Progress(long id)
    {
        var order = await _db.RepairOrders.FindAsync(id);
        if (order == null) return ApiResult.Fail("工单不存在", 404);
        if (UserRole == "student" && order.UserId != UserId) return ApiResult.Fail("无权查看", 403);
        var list = await _db.RepairProgresses.Where(p => p.RepairId == id).OrderBy(p => p.CreateTime).ToListAsync();
        return ApiResult.Ok(list);
    }

    // 学生对已完工工单评价
    [HttpPost("{id}/review"), Authorize(Roles = "student")]
    public async Task<ApiResult> Review(long id, RepairReviewReq req)
    {
        if (req.Rating < 1 || req.Rating > 5) return ApiResult.Fail("评分需在 1-5 星之间");
        var order = await _db.RepairOrders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId);
        if (order == null) return ApiResult.Fail("工单不存在", 404);
        if (order.Status != "done") return ApiResult.Fail("工单完工后才能评价");
        if (order.Rating != null) return ApiResult.Fail("该工单已评价");
        order.Rating = req.Rating; order.Comment = req.Comment; order.ReviewTime = DateTime.Now;
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { message = "评价成功，感谢您的反馈" });
    }
}

// ============ 失物招领（含 AI 语义匹配） ============

[Route("api/lostfound"), Authorize]
public class LostFoundController : BaseController
{
    private readonly AppDbContext _db;
    private readonly AiService _ai;
    public LostFoundController(AppDbContext db, AiService ai) { _db = db; _ai = ai; }

    // 发布时 AI 自动匹配对面类型的帖子（发 lost 匹配 found，反之亦然）
    [HttpPost]
    public async Task<ApiResult> Publish(LostFoundReq req)
    {
        var item = new LostFound
        {
            UserId = UserId, Type = req.Type, Title = req.Title,
            Description = req.Description, Location = req.Location,
            Contact = req.Contact, Image = req.Image
        };
        _db.LostFounds.Add(item);
        await _db.SaveChangesAsync();

        var oppositeType = req.Type == "lost" ? "found" : "lost";
        var candidates = await _db.LostFounds.Where(x => x.Type == oppositeType && x.Status == "active" && x.Id != item.Id).ToListAsync();
        var matches = await _ai.MatchLostFoundAsync(req.Title + " " + req.Description, candidates);

        return ApiResult.Ok(new
        {
            item.Id,
            matches = matches.Select(m => new { m.item.Id, m.item.Title, m.item.Description, m.item.Location, m.item.Contact, similarity = Math.Round(m.score, 2) })
        });
    }

    [HttpGet]
    public async Task<ApiResult> List([FromQuery] string? type)
    {
        var q = _db.LostFounds.Where(x => x.Status == "active");
        if (!string.IsNullOrWhiteSpace(type)) q = q.Where(x => x.Type == type);
        var list = await (from x in q
                          join u in _db.SysUsers on x.UserId equals u.Id
                          orderby x.CreateTime descending
                          select new { x.Id, x.Type, x.Title, x.Description, x.Location, x.Contact, x.Image, x.CreateTime, Publisher = u.Name }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpPost("{id}/close")]
    public async Task<ApiResult> Close(long id)
    {
        var item = await _db.LostFounds.FindAsync(id);
        if (item == null || (item.UserId != UserId && UserRole != "admin"))
            return ApiResult.Fail("无权操作", 403);
        item.Status = "closed";
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

// ============ 校园活动 ============

[Route("api/activities"), Authorize]
public class ActivityController : BaseController
{
    private readonly AppDbContext _db;
    public ActivityController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ApiResult> List()
    {
        var list = await _db.CampusActivities.Where(a => a.Status == "published")
            .OrderByDescending(a => a.CreateTime)
            .Select(a => new
            {
                a.Id, a.Title, a.Content, a.Location, a.StartTime, a.Quota,
                Signed = _db.ActivitySignups.Count(s => s.ActivityId == a.Id && s.Status == "signed"),
                MySigned = _db.ActivitySignups.Any(s => s.ActivityId == a.Id && s.UserId == UserId && s.Status == "signed")
            }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpGet("manage"), Authorize(Roles = "admin,teacher")]
    public async Task<ApiResult> Manage()
    {
        var list = await _db.CampusActivities.OrderByDescending(a => a.CreateTime)
            .Select(a => new { a.Id, a.Title, a.Location, a.StartTime, a.Quota, a.Status, Signed = _db.ActivitySignups.Count(s => s.ActivityId == a.Id && s.Status == "signed") }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpPost, Authorize(Roles = "admin,teacher")]
    public async Task<ApiResult> Create(ActivityReq req)
    {
        var a = new CampusActivity { Title = req.Title, Content = req.Content, Location = req.Location, StartTime = req.StartTime, EndTime = req.EndTime, Quota = req.Quota, Status = req.Status, CreatorId = UserId };
        _db.CampusActivities.Add(a);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { a.Id });
    }

    [HttpPost("{id}/signup"), Authorize(Roles = "student")]
    public async Task<ApiResult> Signup(long id)
    {
        var a = await _db.CampusActivities.FindAsync(id);
        if (a == null || a.Status != "published") return ApiResult.Fail("活动不存在或未发布", 404);
        var signedCount = await _db.ActivitySignups.CountAsync(s => s.ActivityId == id && s.Status == "signed");
        if (signedCount >= a.Quota) return ApiResult.Fail("名额已满");

        var exist = await _db.ActivitySignups.FirstOrDefaultAsync(s => s.ActivityId == id && s.UserId == UserId);
        if (exist != null)
        {
            if (exist.Status == "signed") return ApiResult.Fail("已报名");
            exist.Status = "signed";
        }
        else _db.ActivitySignups.Add(new ActivitySignup { ActivityId = id, UserId = UserId });
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpPost("{id}/cancel"), Authorize(Roles = "student")]
    public async Task<ApiResult> Cancel(long id)
    {
        var s = await _db.ActivitySignups.FirstOrDefaultAsync(x => x.ActivityId == id && x.UserId == UserId && x.Status == "signed");
        if (s == null) return ApiResult.Fail("未报名", 404);
        s.Status = "cancelled";
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }
}

// ============ 场地预约 ============

[Route("api/venues"), Authorize]
public class VenueController : BaseController
{
    private readonly AppDbContext _db;
    public VenueController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ApiResult> List() => ApiResult.Ok(await _db.VenueResources.ToListAsync());

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<ApiResult> Create(VenueReq req)
    {
        var v = new VenueResource { Name = req.Name, Location = req.Location, Capacity = req.Capacity, Equipment = req.Equipment };
        _db.VenueResources.Add(v);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { v.Id });
    }

    [HttpPost("bookings"), Authorize(Roles = "student")]
    public async Task<ApiResult> Book(BookingReq req)
    {
        // 时间冲突检测
        var conflict = await _db.VenueBookings.AnyAsync(b =>
            b.VenueId == req.VenueId && b.Status != "rejected" &&
            b.StartTime < req.EndTime && b.EndTime > req.StartTime);
        if (conflict) return ApiResult.Fail("该时间段已被预约");

        _db.VenueBookings.Add(new VenueBooking { VenueId = req.VenueId, UserId = UserId, StartTime = req.StartTime, EndTime = req.EndTime, Reason = req.Reason });
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpGet("bookings")]
    public async Task<ApiResult> Bookings()
    {
        var q = _db.VenueBookings.AsQueryable();
        if (UserRole == "student") q = q.Where(b => b.UserId == UserId);
        var list = await (from b in q
                          join v in _db.VenueResources on b.VenueId equals v.Id
                          join u in _db.SysUsers on b.UserId equals u.Id
                          orderby b.CreateTime descending
                          select new { b.Id, VenueName = v.Name, UserName = u.Name, b.StartTime, b.EndTime, b.Reason, b.Status }).ToListAsync();
        return ApiResult.Ok(list);
    }

    [HttpPost("bookings/{id}/review"), Authorize(Roles = "admin,teacher")]
    public async Task<ApiResult> Review(long id, BookingReviewReq req)
    {
        var b = await _db.VenueBookings.FindAsync(id);
        if (b == null) return ApiResult.Fail("预约不存在", 404);
        b.Status = req.Approve ? "approved" : "rejected";
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { b.Status });
    }
}

// ============ 统计 ============

[Route("api/stats"), Authorize]
public class StatsController : BaseController
{
    private readonly AppDbContext _db;
    public StatsController(AppDbContext db) { _db = db; }

    // 管理员：平台总览
    [HttpGet("dashboard"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Dashboard()
    {
        return ApiResult.Ok(new
        {
            userCount = await _db.SysUsers.CountAsync(),
            studentCount = await _db.SysUsers.CountAsync(u => u.Role == "student"),
            teacherCount = await _db.SysUsers.CountAsync(u => u.Role == "teacher"),
            logisticsCount = await _db.SysUsers.CountAsync(u => u.Role == "logistics"),
            pendingRepairs = await _db.RepairOrders.CountAsync(r => r.Status == "pending"),
            processingRepairs = await _db.RepairOrders.CountAsync(r => r.Status == "processing"),
            doneRepairs = await _db.RepairOrders.CountAsync(r => r.Status == "done"),
            activityCount = await _db.CampusActivities.CountAsync(a => a.Status == "published"),
            alertCount = await _db.AlertRecords.CountAsync(),
            chatCount = await _db.AiChatLogs.CountAsync(),
            lostFoundCount = await _db.LostFounds.CountAsync(x => x.Status == "active"),
            pendingBookings = await _db.VenueBookings.CountAsync(b => b.Status == "pending"),
            announcementCount = await _db.Announcements.CountAsync(a => a.Status == "published"),
            pendingAnnouncements = await _db.Announcements.CountAsync(a => a.Status == "pending"),
            consultationCount = await _db.Consultations.CountAsync(),
            todayLogins = await _db.LoginLogs.CountAsync(l => l.Result == "success" && l.CreateTime >= DateTime.Today)
        });
    }

    // 管理员：报修类型分布 + 近7天趋势
    [HttpGet("repair-report"), Authorize(Roles = "admin,logistics")]
    public async Task<ApiResult> RepairReport()
    {
        var byCategory = await _db.RepairOrders.GroupBy(r => r.Category)
            .Select(g => new { Name = g.Key ?? "其他", Count = g.Count() }).ToListAsync();
        var byStatus = await _db.RepairOrders.GroupBy(r => r.Status)
            .Select(g => new { Name = g.Key, Count = g.Count() }).ToListAsync();
        var byPriority = await _db.RepairOrders.GroupBy(r => r.Priority)
            .Select(g => new { Name = g.Key ?? "未定", Count = g.Count() }).ToListAsync();

        var raw = await _db.RepairOrders.Where(r => r.CreateTime >= DateTime.Today.AddDays(-6))
            .Select(r => r.CreateTime).ToListAsync();
        var trend = Enumerable.Range(0, 7).Select(i =>
        {
            var d = DateTime.Today.AddDays(-6 + i);
            return new { Date = d.ToString("MM-dd"), Count = raw.Count(x => x.Date == d) };
        }).ToList();

        return ApiResult.Ok(new { byCategory, byStatus, byPriority, trend });
    }

    // 后勤：工单统计（待接单、完成率、平均评分）
    [HttpGet("logistics"), Authorize(Roles = "logistics,admin")]
    public async Task<ApiResult> Logistics()
    {
        var total = await _db.RepairOrders.CountAsync();
        var pending = await _db.RepairOrders.CountAsync(r => r.Status == "pending");
        var processing = await _db.RepairOrders.CountAsync(r => r.Status == "processing");
        var done = await _db.RepairOrders.CountAsync(r => r.Status == "done");
        var scores = await _db.RepairOrders.Where(r => r.Rating != null).Select(r => r.Rating!.Value).ToListAsync();
        var byCategory = await _db.RepairOrders.GroupBy(r => r.Category)
            .Select(g => new { Name = g.Key ?? "其他", Count = g.Count() }).ToListAsync();
        return ApiResult.Ok(new
        {
            total, pending, processing, done,
            completionRate = total == 0 ? 0 : Math.Round(done * 100.0 / total, 1),
            avgRating = scores.Count == 0 ? 0 : Math.Round(scores.Average(), 1),
            reviewCount = scores.Count,
            byCategory
        });
    }

    // 学生：个人概览
    [HttpGet("student"), Authorize(Roles = "student")]
    public async Task<ApiResult> Student()
    {
        return ApiResult.Ok(new
        {
            myRepairs = await _db.RepairOrders.CountAsync(r => r.UserId == UserId),
            pendingRepairs = await _db.RepairOrders.CountAsync(r => r.UserId == UserId && r.Status != "done" && r.Status != "rejected"),
            myAlerts = await _db.AlertRecords.CountAsync(a => a.StudentId == UserId && !a.IsRead),
            unreadMessages = await _db.Notifications.CountAsync(n => n.UserId == UserId && !n.IsRead),
            myBookings = await _db.VenueBookings.CountAsync(b => b.UserId == UserId)
        });
    }
}
