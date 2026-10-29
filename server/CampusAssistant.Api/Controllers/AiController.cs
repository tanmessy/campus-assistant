using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using CampusAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusAssistant.Api.Controllers;

[Route("api/ai"), Authorize]
public class AiController : BaseController
{
    private readonly AppDbContext _db;
    private readonly AiService _ai;
    public AiController(AppDbContext db, AiService ai) { _db = db; _ai = ai; }

    // RAG 问答：知识库检索 + 大模型生成（支持多轮上下文）+ 记录入库
    [HttpPost("chat")]
    public async Task<ApiResult> Chat(ChatReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Question)) return ApiResult.Fail("问题不能为空");
        var history = req.History?.Select(h => (h.Role, h.Text)).ToList();
        var answer = await _ai.RagChatAsync(_db, req.Question, history);
        _db.AiChatLogs.Add(new AiChatLog { UserId = UserId, Question = req.Question, Answer = answer });
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { answer, aiEnabled = _ai.HasKey });
    }

    [HttpGet("history")]
    public async Task<ApiResult> History()
    {
        var list = await _db.AiChatLogs.Where(x => x.UserId == UserId)
            .OrderByDescending(x => x.CreateTime).Take(50).ToListAsync();
        return ApiResult.Ok(list.OrderBy(x => x.CreateTime));
    }

    // 报修描述智能预分析（提交前可调用预览）
    [HttpPost("classify-repair"), Authorize(Roles = "student")]
    public async Task<ApiResult> ClassifyRepair([FromBody] ChatReq req)
    {
        var (cat, pri, advice) = await _ai.ClassifyRepairAsync(req.Question);
        return ApiResult.Ok(new { category = cat, priority = pri, advice });
    }

    // AI 学习建议：基于本人不及格成绩生成
    [HttpGet("study-advice"), Authorize(Roles = "student")]
    public async Task<ApiResult> StudyAdvice()
    {
        var failed = await (from s in _db.Scores
                            join c in _db.Courses on s.CourseId equals c.Id
                            where s.StudentId == UserId && s.Value < 60
                            select new { c.Name, s.Value }).ToListAsync();
        var me = await _db.SysUsers.FindAsync(UserId);
        var advice = await _ai.StudyAdviceAsync(me!.Name, failed.Select(f => (f.Name, f.Value)).ToList());
        return ApiResult.Ok(new { advice, failedCount = failed.Count, aiEnabled = _ai.HasKey });
    }

    // ---- 知识库管理（管理员） ----

    [HttpGet("knowledge")]
    public async Task<ApiResult> KnowledgeList() =>
        ApiResult.Ok(await _db.AiKnowledges.OrderByDescending(x => x.CreateTime).ToListAsync());

    [HttpPost("knowledge"), Authorize(Roles = "admin")]
    public async Task<ApiResult> KnowledgeCreate(KnowledgeReq req)
    {
        var k = new AiKnowledge { Title = req.Title, Content = req.Content, Category = req.Category, IsEnabled = req.IsEnabled };
        _db.AiKnowledges.Add(k);
        await _db.SaveChangesAsync();
        return ApiResult.Ok(new { k.Id });
    }

    [HttpPut("knowledge/{id}"), Authorize(Roles = "admin")]
    public async Task<ApiResult> KnowledgeUpdate(long id, KnowledgeReq req)
    {
        var k = await _db.AiKnowledges.FindAsync(id);
        if (k == null) return ApiResult.Fail("条目不存在", 404);
        k.Title = req.Title; k.Content = req.Content; k.Category = req.Category; k.IsEnabled = req.IsEnabled;
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    [HttpDelete("knowledge/{id}"), Authorize(Roles = "admin")]
    public async Task<ApiResult> KnowledgeDelete(long id)
    {
        var k = await _db.AiKnowledges.FindAsync(id);
        if (k == null) return ApiResult.Fail("条目不存在", 404);
        _db.AiKnowledges.Remove(k);
        await _db.SaveChangesAsync();
        return ApiResult.Ok();
    }

    // 全部对话记录（管理员）
    [HttpGet("logs"), Authorize(Roles = "admin")]
    public async Task<ApiResult> Logs()
    {
        var list = await (from l in _db.AiChatLogs
                          join u in _db.SysUsers on l.UserId equals u.Id
                          orderby l.CreateTime descending
                          select new { l.Id, UserName = u.Name, l.Question, l.Answer, l.CreateTime }).Take(200).ToListAsync();
        return ApiResult.Ok(list);
    }

    // AI 生成活动宣传文案（管理端 AIGC）
    [HttpPost("generate-activity-copy"), Authorize(Roles = "admin,teacher")]
    public async Task<ApiResult> GenerateActivityCopy(ActivityCopyReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Title)) return ApiResult.Fail("请先填写活动标题");
        var copy = await _ai.GenerateActivityCopyAsync(req.Title, req.Location, req.Time, req.Extra);
        return ApiResult.Ok(new { copy, aiEnabled = _ai.HasKey });
    }

    // 仪表盘 AI 数据摘要
    [HttpGet("dashboard-summary"), Authorize(Roles = "admin")]
    public async Task<ApiResult> DashboardSummary()
    {
        var summary = await _ai.DashboardSummaryAsync(_db);
        return ApiResult.Ok(new { summary, aiEnabled = _ai.HasKey });
    }
}
