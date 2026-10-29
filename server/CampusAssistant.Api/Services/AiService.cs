using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CampusAssistant.Api.Data;
using CampusAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusAssistant.Api.Services;

/// <summary>
/// AI 服务：接入智谱 GLM（OpenAI 兼容协议，glm-4-flash 免费模型）。
/// 六大智能化能力：
/// 1) RAG 校园问答（LLM 语义重排 + 多轮上下文）
/// 2) 报修智能分类
/// 3) 失物招领语义匹配
/// 4) 学业预警 AI 学习建议
/// 5) 管理端 AI 活动文案生成（AIGC）
/// 6) 仪表盘 AI 数据摘要
/// 未配置 ApiKey 或调用失败时自动降级为本地规则/关键词逻辑，演示不中断。
/// 说明：语义检索不依赖付费 Embedding 接口，改用免费对话模型做候选重排，零成本。
/// </summary>
public class AiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    private readonly ILogger<AiService> _log;

    public AiService(IHttpClientFactory factory, IConfiguration cfg, ILogger<AiService> log)
    {
        _http = factory.CreateClient("ai");
        _cfg = cfg;
        _log = log;
    }

    public bool HasKey => !string.IsNullOrWhiteSpace(_cfg["Ai:ApiKey"]);
    private string BaseUrl => _cfg["Ai:BaseUrl"] ?? "https://open.bigmodel.cn/api/paas/v4";
    private string Model => _cfg["Ai:Model"] ?? "glm-4-flash";

    // ---------------- 底层调用 ----------------

    private async Task<string?> ChatAsync(List<object> messages, double temperature = 0.7)
    {
        if (!HasKey) return null;
        // 免费模型并发受限，失败时重试一次
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                var body = new { model = Model, messages, temperature };
                var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/chat/completions")
                {
                    Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
                };
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cfg["Ai:ApiKey"]);
                var resp = await _http.SendAsync(req);
                resp.EnsureSuccessStatusCode();
                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            }
            catch (Exception ex)
            {
                _log.LogWarning("大模型调用失败（第{n}次）：{msg}", attempt + 1, ex.Message);
                if (attempt == 0) await Task.Delay(1500);
            }
        }
        return null;
    }

    private Task<string?> ChatAsync(string system, string user, double temperature = 0.7) =>
        ChatAsync(new List<object>
        {
            new { role = "system", content = system },
            new { role = "user", content = user }
        }, temperature);

    // ---------------- 能力1：RAG 校园问答（多轮上下文） ----------------

    public async Task<string> RagChatAsync(AppDbContext db, string question, List<(string role, string text)>? history = null)
    {
        var kb = await db.AiKnowledges.Where(k => k.IsEnabled).ToListAsync();
        var top = await RetrieveTopKnowledgeAsync(kb, question, 3);

        if (HasKey)
        {
            var context = string.Join("\n\n", top.Select(k => $"【{k.Title}】{k.Content}"));
            var system = "你是高校校园生活智能助手。请基于以下校园知识库内容回答学生问题，回答要简洁、口语化、分点说明。" +
                         "如果知识库中没有相关内容，请诚实说明\"知识库中暂时没有该信息\"，并给出一般性建议，不要编造学校政策。" +
                         "你可以结合对话历史理解用户的追问（比如\"那呢\"\"还有吗\"这类省略指代）。\n\n知识库内容：\n" +
                         (string.IsNullOrWhiteSpace(context) ? "（空）" : context);

            // 组装多轮消息：system + 最近 6 条历史 + 当前问题
            var messages = new List<object> { new { role = "system", content = system } };
            if (history != null)
                foreach (var (role, text) in history.TakeLast(6))
                    messages.Add(new { role = role == "user" ? "user" : "assistant", content = text });
            messages.Add(new { role = "user", content = question });

            var answer = await ChatAsync(messages);
            if (!string.IsNullOrWhiteSpace(answer)) return answer;
        }

        // 降级：直接拼接最相关知识
        if (top.Count > 0)
            return $"根据校园知识库：\n\n【{top[0].Title}】\n{top[0].Content}\n\n（大模型暂时不可用，以上为知识库原文检索结果）";
        return "抱歉，知识库中暂未收录该问题，请联系管理员补充知识，或前往 PC 管理端录入相关条目。";
    }

    /// <summary>
    /// 语义检索：把候选知识编号后交给免费对话模型做语义相关性重排，
    /// 不依赖付费 Embedding 接口；失败时降级为关键词重叠度打分。
    /// </summary>
    private async Task<List<AiKnowledge>> RetrieveTopKnowledgeAsync(List<AiKnowledge> kb, string question, int n)
    {
        if (kb.Count == 0) return new();

        if (HasKey && kb.Count > n)
        {
            var sb = new StringBuilder("以下是校园知识库条目：\n");
            for (int i = 0; i < kb.Count; i++)
                sb.AppendLine($"{i + 1}. {kb[i].Title}：{Truncate(kb[i].Content, 60)}");
            sb.AppendLine($"\n学生问题：{question}");
            sb.AppendLine($"请选出与该问题语义最相关的最多 {n} 条，只输出编号，用逗号分隔（如：1,3）。若都不相关，输出：无");

            var raw = await ChatAsync("你是检索助手，只做相关性判断，严格按用户要求的格式输出。", sb.ToString(), 0.1);
            var picked = ParseIndices(raw, kb.Count);
            if (picked.Count > 0)
                return picked.Take(n).Select(i => kb[i]).ToList();
        }

        // 降级：关键词重叠打分
        return kb.Select(k => (k, s: KeywordScore(question, k.Title + " " + k.Content)))
                 .OrderByDescending(x => x.s).Take(n).Where(x => x.s > 0)
                 .Select(x => x.k).ToList();
    }

    // ---------------- 能力2：报修智能分类 ----------------

    public async Task<(string category, string priority, string advice)> ClassifyRepairAsync(string description)
    {
        if (HasKey && !string.IsNullOrWhiteSpace(description))
        {
            var system = "你是高校后勤报修工单分析助手。根据学生的报修描述，输出 JSON（不要输出其他内容）：" +
                         "{\"category\":\"水电/门窗/网络/家电/家具/其他 六选一\",\"priority\":\"高/中/低\"," +
                         "\"advice\":\"给维修师傅的一句维修建议或给学生的一句临时处理建议\"}。" +
                         "水电=水管/电路/开关/灯具/插座；家电=空调/热水器/洗衣机/饮水机；" +
                         "漏水、漏电、冒烟、无法开门等涉及安全的为高优先级；影响日常生活的为中；其他为低。";
            var raw = await ChatAsync(system, description, 0.2);
            if (raw != null)
            {
                try
                {
                    var start = raw.IndexOf('{'); var end = raw.LastIndexOf('}');
                    if (start >= 0 && end > start)
                    {
                        using var doc = JsonDocument.Parse(raw[start..(end + 1)]);
                        var rawCat = doc.RootElement.GetProperty("category").GetString() ?? "";
                        // 白名单清洗：模型偶尔会把所有选项都输出，取命中的第一个合法分类
                        string[] valid = { "水电", "门窗", "网络", "家电", "家具", "其他" };
                        var aiCat = valid.FirstOrDefault(v => rawCat.Contains(v)) ?? "其他";
                        var aiPri = doc.RootElement.GetProperty("priority").GetString() ?? "中";
                        if (aiPri != "高" && aiPri != "中" && aiPri != "低") aiPri = "中";
                        return (aiCat, aiPri, doc.RootElement.GetProperty("advice").GetString() ?? "");
                    }
                }
                catch { /* 解析失败走降级 */ }
            }
        }

        var d = description ?? "";
        string cat = d.Contains("水") || d.Contains("电") || d.Contains("灯") || d.Contains("漏") || d.Contains("插座") ? "水电"
                   : d.Contains("空调") || d.Contains("热水器") || d.Contains("洗衣机") || d.Contains("饮水机") ? "家电"
                   : d.Contains("门") || d.Contains("窗") || d.Contains("锁") ? "门窗"
                   : d.Contains("网") || d.Contains("wifi", StringComparison.OrdinalIgnoreCase) ? "网络"
                   : d.Contains("床") || d.Contains("桌") || d.Contains("椅") ? "家具" : "其他";
        string pri = d.Contains("漏") || d.Contains("冒烟") || d.Contains("跳闸") || d.Contains("打不开") ? "高" : "中";
        return (cat, pri, "请后勤师傅携带常规工具上门处理。");
    }

    // ---------------- 能力3：失物招领语义匹配 ----------------

    /// <summary>返回 (匹配项, 相似度) 列表，相似度 0-1。</summary>
    public async Task<List<(LostFound item, double score)>> MatchLostFoundAsync(string text, List<LostFound> candidates)
    {
        if (candidates.Count == 0) return new();
        var pool = candidates.OrderByDescending(c => c.CreateTime).Take(20).ToList();

        if (HasKey)
        {
            var sb = new StringBuilder("以下是失物招领平台上的帖子：\n");
            for (int i = 0; i < pool.Count; i++)
                sb.AppendLine($"{i + 1}. {pool[i].Title}｜{Truncate(pool[i].Description, 40)}｜地点:{pool[i].Location}");
            sb.AppendLine($"\n新发布的信息：{text}");
            sb.AppendLine("请判断哪些帖子与新信息描述的可能是同一件物品（语义相近即可，不要求字面相同），" +
                          "只输出可能匹配的编号，用逗号分隔，按可能性从高到低排序，最多5个。若都不相关，输出：无");

            var raw = await ChatAsync("你是失物招领匹配助手，只做匹配判断，严格按格式输出。", sb.ToString(), 0.1);
            var picked = ParseIndices(raw, pool.Count);
            if (picked.Count > 0)
                return picked.Take(5).Select((idx, rank) => (pool[idx], 0.95 - rank * 0.07)).ToList();
        }

        return pool.Select(c => (c, s: KeywordScore(text, c.Title + " " + c.Description + " " + c.Location)))
                   .Where(x => x.s > 0).OrderByDescending(x => x.s).Take(5).ToList();
    }

    // ---------------- 能力4：AI 学业预警学习建议 ----------------

    public async Task<string> StudyAdviceAsync(string studentName, List<(string course, decimal score)> failed)
    {
        if (failed.Count == 0) return "目前没有不及格科目，继续保持！";
        var list = string.Join("、", failed.Select(f => $"{f.course}({f.score}分)"));

        if (HasKey)
        {
            var system = "你是高校学业导师。根据学生的不及格科目和分数，给出 3-5 条具体、可执行的学习建议，" +
                         "语气鼓励、不说教，每条一行，总字数 200 字以内。";
            var answer = await ChatAsync(system, $"学生{studentName}的不及格科目：{list}");
            if (!string.IsNullOrWhiteSpace(answer)) return answer;
        }
        return $"你在 {list} 科目上成绩低于 60 分。建议：1. 优先复习分数最低的科目；2. 联系任课老师答疑；3. 与成绩较好的同学组成学习小组。";
    }

    // ---------------- 能力5：AI 活动文案生成（管理端 AIGC） ----------------

    public async Task<string> GenerateActivityCopyAsync(string title, string? location, string? time, string? extra)
    {
        var info = $"活动名称：{title}；地点：{location ?? "待定"}；时间：{time ?? "待定"}；补充说明：{extra ?? "无"}";
        if (HasKey)
        {
            var system = "你是高校团委宣传干事。根据活动信息撰写一段活动宣传文案，150 字左右，语言青春活泼、有号召力，" +
                         "包含活动亮点和报名方式提示，可适当分段。只输出文案正文。";
            var copy = await ChatAsync(system, info);
            if (!string.IsNullOrWhiteSpace(copy)) return copy;
        }
        return $"{title}即将开始！地点：{location ?? "待定"}。欢迎同学们踊跃报名，名额有限，先到先得！";
    }

    // ---------------- 能力6：仪表盘 AI 数据摘要 ----------------

    public async Task<string> DashboardSummaryAsync(AppDbContext db)
    {
        var stats = new
        {
            用户数 = await db.SysUsers.CountAsync(),
            待处理工单 = await db.RepairOrders.CountAsync(r => r.Status == "pending"),
            处理中工单 = await db.RepairOrders.CountAsync(r => r.Status == "processing"),
            进行中活动 = await db.CampusActivities.CountAsync(a => a.Status == "published"),
            学业预警 = await db.AlertRecords.CountAsync(a => !a.IsRead),
            待审预约 = await db.VenueBookings.CountAsync(b => b.Status == "pending"),
            AI对话次数 = await db.AiChatLogs.CountAsync(),
            今日新增工单 = await db.RepairOrders.CountAsync(r => r.CreateTime.Date == DateTime.Today)
        };

        if (HasKey)
        {
            var system = "你是校园管理平台的数据分析助手。根据平台实时统计数据，生成 3-4 条管理摘要：" +
                         "指出需要优先关注的事项、发现的趋势，并给出一句可执行的管理建议。每条一行，总字数 150 字以内。";
            var summary = await ChatAsync(system, JsonSerializer.Serialize(stats));
            if (!string.IsNullOrWhiteSpace(summary)) return summary;
        }
        return $"当前有待处理工单 {stats.待处理工单} 条、待审核预约 {stats.待审预约} 条、未读预警 {stats.学业预警} 条，请优先处理。";
    }

    // ---------------- 工具 ----------------

    /// <summary>从模型输出中解析编号列表（"1,3" / "1、3" / "无"），返回 0-based 下标。</summary>
    private static List<int> ParseIndices(string? raw, int max)
    {
        var result = new List<int>();
        if (string.IsNullOrWhiteSpace(raw) || raw.Contains('无')) return result;
        foreach (var token in raw.Split(new[] { ',', '，', '、', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var digits = new string(token.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var n) && n >= 1 && n <= max && !result.Contains(n - 1))
                result.Add(n - 1);
        }
        return result;
    }

    private static double KeywordScore(string query, string doc)
    {
        var terms = query.Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                         .GroupBy(c => c).Select(g => g.Key).ToList();
        if (terms.Count == 0) return 0;
        double hit = terms.Count(t => doc.Contains(t));
        return hit / terms.Count;
    }

    private static string Truncate(string? s, int len) =>
        string.IsNullOrEmpty(s) ? "" : s.Length <= len ? s : s[..len] + "…";
}
