namespace CampusAssistant.Api.Models;

// 统一返回格式：{ code, message, data }
public record ApiResult(int Code, string Message, object? Data)
{
    public static ApiResult Ok(object? data = null) => new(200, "success", data);
    public static ApiResult Fail(string message, int code = 400) => new(code, message, null);
}

// 统一分页返回：总记录数 + 当前页数据 + 分页元信息
public record PagedResult<T>(int Total, int PageIndex, int PageSize, int TotalPages, List<T> List)
{
    public static PagedResult<T> Of(IEnumerable<T> all, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex < 1 ? 1 : pageIndex;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);
        var total = all.Count();
        var list = all.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        var pages = (int)Math.Ceiling(total / (double)pageSize);
        return new PagedResult<T>(total, pageIndex, pageSize, pages, list);
    }
}

public record LoginReq(string Username, string Password);
public record RegisterReq(string Username, string Password, string Name, string? StudentNo, string? Phone);
public record ChangePwdReq(string OldPassword, string NewPassword);
public record UpdateProfileReq(string Name, string? Phone, string? Email);
public record CreateUserReq(string Username, string Name, string Role, string? StudentNo, string? Phone);
public record StatusReq(string Status); // enabled / disabled
public record ChatReq(string Question, List<ChatMsg>? History);
public record ChatMsg(string Role, string Text);
public record ActivityCopyReq(string Title, string? Location, string? Time, string? Extra);
public record RepairReq(string? Category, string? Dormitory, string? Description, string? Images, string? Contact);
public record RepairHandleReq(string Action, string? Remark, string? Images, long? AssignTo); // accept/assign/done/reject
public record RepairReviewReq(int Rating, string? Comment);
public record RepairProgressReq(string Status, string? Remark, string? Images);
public record LostFoundReq(string Type, string Title, string? Description, string? Location, string? Contact, string? Image);
public record ActivityReq(string Title, string? Content, string? Location, DateTime? StartTime, DateTime? EndTime, int Quota, string Status);
public record VenueReq(string Name, string? Location, int Capacity, string? Equipment);
public record BookingReq(long VenueId, DateTime StartTime, DateTime EndTime, string? Reason);
public record BookingReviewReq(bool Approve);
public record ScoreReq(long StudentId, long CourseId, decimal Value, string? Semester);
public record KnowledgeReq(string Title, string? Content, string? Category, bool IsEnabled);
public record CourseReq(string Name, long TeacherId, string? Classroom, string? Schedule, string? Semester, int Credit);
public record AnnouncementReq(string Title, string? Content, string Category, string Scope, string Status, bool IsTop);
public record AnnouncementAuditReq(bool Approve, string? Reason);
public record CafeteriaReq(string Canteen, string Window, string MealType, string DishName, decimal Price, string? Description);
public record ConsultationReq(string Question, string Category);
public record ConsultationReplyReq(string Reply);
