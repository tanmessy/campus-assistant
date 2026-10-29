using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusAssistant.Api.Models;

// 表1：sys_user 用户表
public class SysUser
{
    [Key] public long Id { get; set; }
    [MaxLength(50)] public string Username { get; set; } = "";
    [MaxLength(200)] public string Password { get; set; } = "";   // BCrypt
    [MaxLength(50)] public string Name { get; set; } = "";
    [MaxLength(20)] public string Role { get; set; } = "student"; // student/teacher/logistics/admin
    [MaxLength(30)] public string? StudentNo { get; set; }
    [MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(20)] public string? Email { get; set; }
    [MaxLength(20)] public string Status { get; set; } = "enabled"; // enabled/disabled
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;
}

// 表2：course 课程表
public class Course
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    public long TeacherId { get; set; }
    [MaxLength(50)] public string? Classroom { get; set; }
    [MaxLength(20)] public string? Schedule { get; set; }   // 如：周一1-2节
    [MaxLength(20)] public string? Semester { get; set; }
    public int Credit { get; set; } = 2;
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表3：student_course 选课表
public class StudentCourse
{
    [Key] public long Id { get; set; }
    public long StudentId { get; set; }
    public long CourseId { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表4：score 成绩表
public class Score
{
    [Key] public long Id { get; set; }
    public long StudentId { get; set; }
    public long CourseId { get; set; }
    [Column(TypeName = "decimal(5,1)")] public decimal Value { get; set; }
    [MaxLength(20)] public string? Semester { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表5：repair_order 报修工单表
public class RepairOrder
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(50)] public string? Category { get; set; }  // AI 自动分类填充
    [MaxLength(100)] public string? Dormitory { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(1000)] public string? Images { get; set; }  // 逗号分隔 URL
    [MaxLength(20)] public string Status { get; set; } = "pending"; // pending/processing/done/rejected
    [MaxLength(10)] public string? Priority { get; set; }  // AI 预判紧急度：高/中/低
    [MaxLength(500)] public string? AiAdvice { get; set; } // AI 维修建议
    public long? HandlerId { get; set; }
    [MaxLength(20)] public string? Contact { get; set; }   // 联系电话
    public int? Rating { get; set; }                        // 学生评价 1-5 星
    [MaxLength(500)] public string? Comment { get; set; }   // 评价内容
    public DateTime? ReviewTime { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;
}

// 表6：lost_found 失物招领表
public class LostFound
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(10)] public string Type { get; set; } = "lost"; // lost 丢失 / found 招领
    [MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(100)] public string? Location { get; set; }
    [MaxLength(500)] public string? Image { get; set; }
    [MaxLength(30)] public string? Contact { get; set; }
    [MaxLength(10)] public string Status { get; set; } = "active"; // active/closed
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表7：campus_activity 校园活动表
public class CampusActivity
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(2000)] public string? Content { get; set; }
    [MaxLength(100)] public string? Location { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Quota { get; set; } = 50;
    [MaxLength(20)] public string Status { get; set; } = "published"; // draft/published/ended
    public long CreatorId { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表8：activity_signup 活动报名表
public class ActivitySignup
{
    [Key] public long Id { get; set; }
    public long ActivityId { get; set; }
    public long UserId { get; set; }
    [MaxLength(10)] public string Status { get; set; } = "signed"; // signed/cancelled
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表9：venue_resource 场地资源表
public class VenueResource
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(100)] public string? Location { get; set; }
    public int Capacity { get; set; } = 30;
    [MaxLength(200)] public string? Equipment { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表10：venue_booking 场地预约表
public class VenueBooking
{
    [Key] public long Id { get; set; }
    public long VenueId { get; set; }
    public long UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    [MaxLength(200)] public string? Reason { get; set; }
    [MaxLength(10)] public string Status { get; set; } = "pending"; // pending/approved/rejected
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表11：ai_knowledge 知识库表
public class AiKnowledge
{
    [Key] public long Id { get; set; }
    [MaxLength(200)] public string Title { get; set; } = "";
    public string? Content { get; set; }
    [MaxLength(50)] public string? Category { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表12：ai_chat_log 对话记录表
public class AiChatLog
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表13：alert_record 学业预警表
public class AlertRecord
{
    [Key] public long Id { get; set; }
    public long StudentId { get; set; }
    [MaxLength(100)] public string? CourseName { get; set; }
    [Column(TypeName = "decimal(5,1)")] public decimal ScoreValue { get; set; }
    [MaxLength(200)] public string? Reason { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表14：repair_progress 工单进度表（接单/处理/完工全流程留痕）
public class RepairProgress
{
    [Key] public long Id { get; set; }
    public long RepairId { get; set; }
    [MaxLength(20)] public string Status { get; set; } = "pending"; // pending/processing/done/rejected
    [MaxLength(500)] public string? Remark { get; set; }            // 进度说明
    [MaxLength(1000)] public string? Images { get; set; }           // 现场照片
    public long OperatorId { get; set; }
    [MaxLength(50)] public string? OperatorName { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表15：announcement 公告表
public class Announcement
{
    [Key] public long Id { get; set; }
    [MaxLength(200)] public string Title { get; set; } = "";
    public string? Content { get; set; }
    [MaxLength(20)] public string Category { get; set; } = "notice"; // notice 通知 / teaching 教务 / life 生活 / activity 活动
    [MaxLength(20)] public string Scope { get; set; } = "all";       // all 全校 / class 班级 / dept 院系
    [MaxLength(20)] public string Status { get; set; } = "pending";  // pending 待审核 / published 已发布 / rejected 已驳回
    public bool IsTop { get; set; } = false;
    public long PublisherId { get; set; }
    [MaxLength(50)] public string? PublisherName { get; set; }
    public int ViewCount { get; set; } = 0;
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;
}

// 表16：cafeteria_menu 食堂菜单表
public class CafeteriaMenu
{
    [Key] public long Id { get; set; }
    [MaxLength(50)] public string Canteen { get; set; } = "";    // 一食堂 / 二食堂
    [MaxLength(50)] public string Window { get; set; } = "";     // 窗口名称
    [MaxLength(20)] public string MealType { get; set; } = "午餐"; // 早餐/午餐/晚餐
    [MaxLength(100)] public string DishName { get; set; } = "";
    [Column(TypeName = "decimal(6,1)")] public decimal Price { get; set; }
    [MaxLength(200)] public string? Description { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表17：notification 消息表（消息中心）
public class Notification
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(500)] public string? Content { get; set; }
    [MaxLength(20)] public string Type { get; set; } = "system"; // repair 报修 / announce 公告 / ai AI提醒 / system 系统
    public bool IsRead { get; set; } = false;
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表18：login_log 登录日志表
public class LoginLog
{
    [Key] public long Id { get; set; }
    public long? UserId { get; set; }
    [MaxLength(50)] public string Username { get; set; } = "";
    [MaxLength(50)] public string? Ip { get; set; }
    [MaxLength(20)] public string Result { get; set; } = "success"; // success/fail
    [MaxLength(100)] public string? Message { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表19：operation_log 操作日志表
public class OperationLog
{
    [Key] public long Id { get; set; }
    public long? UserId { get; set; }
    [MaxLength(50)] public string? Username { get; set; }
    [MaxLength(50)] public string? Module { get; set; }
    [MaxLength(100)] public string? Action { get; set; }
    [MaxLength(500)] public string? Detail { get; set; }
    [MaxLength(50)] public string? Ip { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

// 表20：consultation 学生咨询表（AI 助手问题提交 + 教师回复）
public class Consultation
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(500)] public string Question { get; set; } = "";
    [MaxLength(20)] public string Category { get; set; } = "学习"; // 学习/生活/心理/其他
    [MaxLength(1000)] public string? Reply { get; set; }
    public long? ReplierId { get; set; }
    [MaxLength(50)] public string? ReplierName { get; set; }
    [MaxLength(20)] public string Status { get; set; } = "pending"; // pending/replied
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime? ReplyTime { get; set; }
}
