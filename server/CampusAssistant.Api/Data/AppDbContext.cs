using CampusAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusAssistant.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
    public DbSet<Score> Scores => Set<Score>();
    public DbSet<RepairOrder> RepairOrders => Set<RepairOrder>();
    public DbSet<LostFound> LostFounds => Set<LostFound>();
    public DbSet<CampusActivity> CampusActivities => Set<CampusActivity>();
    public DbSet<ActivitySignup> ActivitySignups => Set<ActivitySignup>();
    public DbSet<VenueResource> VenueResources => Set<VenueResource>();
    public DbSet<VenueBooking> VenueBookings => Set<VenueBooking>();
    public DbSet<AiKnowledge> AiKnowledges => Set<AiKnowledge>();
    public DbSet<AiChatLog> AiChatLogs => Set<AiChatLog>();
    public DbSet<AlertRecord> AlertRecords => Set<AlertRecord>();
    public DbSet<RepairProgress> RepairProgresses => Set<RepairProgress>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<CafeteriaMenu> CafeteriaMenus => Set<CafeteriaMenu>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<Consultation> Consultations => Set<Consultation>();

    // 初始化种子数据（首次启动时调用）
    public static void Seed(AppDbContext db)
    {
        if (db.SysUsers.Any()) return;

        string Hash(string pwd) => BCrypt.Net.BCrypt.HashPassword(pwd);
        var now = DateTime.Now;

        // ---------------- 用户 ----------------
        var admin = new SysUser { Username = "admin", Password = Hash("123456"), Name = "系统管理员", Role = "admin" };
        var teacher = new SysUser { Username = "teacher", Password = Hash("123456"), Name = "王建国", Role = "teacher", Phone = "13800000001" };
        var teacher2 = new SysUser { Username = "teacher2", Password = Hash("123456"), Name = "李慧敏", Role = "teacher", Phone = "13800000002" };
        var logistics = new SysUser { Username = "logistics", Password = Hash("123456"), Name = "张维修", Role = "logistics", Phone = "13800000003" };
        var stu1 = new SysUser { Username = "student", Password = Hash("123456"), Name = "谭浩杰", Role = "student", StudentNo = "20250751119", Phone = "13900000001" };
        var stu2 = new SysUser { Username = "student2", Password = Hash("123456"), Name = "张三", Role = "student", StudentNo = "20250751120", Phone = "13900000002" };
        var stu3 = new SysUser { Username = "student3", Password = Hash("123456"), Name = "刘思远", Role = "student", StudentNo = "20250751121", Phone = "13900000003" };
        var stu4 = new SysUser { Username = "student4", Password = Hash("123456"), Name = "陈晓雯", Role = "student", StudentNo = "20250751122", Phone = "13900000004" };
        db.SysUsers.AddRange(admin, teacher, teacher2, logistics, stu1, stu2, stu3, stu4);
        db.SaveChanges();

        // ---------------- 课程 / 选课 / 成绩 / 预警 ----------------
        var c1 = new Course { Name = "高等数学(上)", TeacherId = teacher.Id, Classroom = "教一101", Schedule = "周一1-2节", Semester = "2025-2026-1", Credit = 4 };
        var c2 = new Course { Name = "大学英语(三)", TeacherId = teacher2.Id, Classroom = "教二203", Schedule = "周三3-4节", Semester = "2025-2026-1", Credit = 3 };
        var c3 = new Course { Name = "Web 应用开发", TeacherId = teacher.Id, Classroom = "实验楼305", Schedule = "周五5-6节", Semester = "2025-2026-1", Credit = 3 };
        var c4 = new Course { Name = "数据结构", TeacherId = teacher.Id, Classroom = "教三402", Schedule = "周二7-8节", Semester = "2025-2026-1", Credit = 4 };
        var c5 = new Course { Name = "大学物理(下)", TeacherId = teacher2.Id, Classroom = "教一305", Schedule = "周四3-4节", Semester = "2025-2026-1", Credit = 3 };
        db.Courses.AddRange(c1, c2, c3, c4, c5);
        db.SaveChanges();

        db.StudentCourses.AddRange(
            new StudentCourse { StudentId = stu1.Id, CourseId = c1.Id },
            new StudentCourse { StudentId = stu1.Id, CourseId = c2.Id },
            new StudentCourse { StudentId = stu1.Id, CourseId = c3.Id },
            new StudentCourse { StudentId = stu1.Id, CourseId = c4.Id },
            new StudentCourse { StudentId = stu2.Id, CourseId = c1.Id },
            new StudentCourse { StudentId = stu2.Id, CourseId = c4.Id },
            new StudentCourse { StudentId = stu3.Id, CourseId = c1.Id },
            new StudentCourse { StudentId = stu3.Id, CourseId = c2.Id },
            new StudentCourse { StudentId = stu3.Id, CourseId = c5.Id },
            new StudentCourse { StudentId = stu4.Id, CourseId = c2.Id },
            new StudentCourse { StudentId = stu4.Id, CourseId = c3.Id });

        db.Scores.AddRange(
            new Score { StudentId = stu1.Id, CourseId = c1.Id, Value = 88, Semester = "2025-2026-1" },
            new Score { StudentId = stu1.Id, CourseId = c2.Id, Value = 55, Semester = "2025-2026-1" },
            new Score { StudentId = stu1.Id, CourseId = c3.Id, Value = 92, Semester = "2025-2026-1" },
            new Score { StudentId = stu1.Id, CourseId = c4.Id, Value = 58, Semester = "2025-2026-1" },
            new Score { StudentId = stu2.Id, CourseId = c1.Id, Value = 42, Semester = "2025-2026-1" },
            new Score { StudentId = stu2.Id, CourseId = c4.Id, Value = 71, Semester = "2025-2026-1" },
            new Score { StudentId = stu3.Id, CourseId = c1.Id, Value = 79, Semester = "2025-2026-1" },
            new Score { StudentId = stu3.Id, CourseId = c2.Id, Value = 83, Semester = "2025-2026-1" },
            new Score { StudentId = stu3.Id, CourseId = c5.Id, Value = 47, Semester = "2025-2026-1" },
            new Score { StudentId = stu4.Id, CourseId = c2.Id, Value = 90, Semester = "2025-2026-1" },
            new Score { StudentId = stu4.Id, CourseId = c3.Id, Value = 85, Semester = "2025-2026-1" });

        db.AlertRecords.AddRange(
            new AlertRecord { StudentId = stu1.Id, CourseName = "大学英语(三)", ScoreValue = 55, Reason = "成绩低于60分", CreateTime = now.AddDays(-3) },
            new AlertRecord { StudentId = stu1.Id, CourseName = "数据结构", ScoreValue = 58, Reason = "成绩低于60分", CreateTime = now.AddDays(-1) },
            new AlertRecord { StudentId = stu2.Id, CourseName = "高等数学(上)", ScoreValue = 42, Reason = "成绩低于60分", IsRead = true, CreateTime = now.AddDays(-5) },
            new AlertRecord { StudentId = stu3.Id, CourseName = "大学物理(下)", ScoreValue = 47, Reason = "成绩低于60分", CreateTime = now.AddDays(-2) });

        // ---------------- 报修工单（覆盖各状态） ----------------
        db.RepairOrders.AddRange(
            new RepairOrder { UserId = stu1.Id, Category = "水电", Dormitory = "东区3栋502", Description = "宿舍洗手池水管漏水，地面积水严重", Status = "pending", Priority = "高", AiAdvice = "建议先关闭入户水阀，师傅携带管钳与生料带上门。", CreateTime = now.AddHours(-2) },
            new RepairOrder { UserId = stu2.Id, Category = "网络", Dormitory = "西区7栋318", Description = "宿舍WiFi信号特别差，经常断连", Status = "processing", Priority = "中", AiAdvice = "建议检查楼道交换机端口，必要时重启AP。", HandlerId = logistics.Id, CreateTime = now.AddDays(-1) },
            new RepairOrder { UserId = stu3.Id, Category = "门窗", Dormitory = "东区5栋210", Description = "阳台推拉门轨道卡死，开关很费力", Status = "done", Priority = "低", AiAdvice = "清理轨道杂物并加润滑油即可。", HandlerId = logistics.Id, CreateTime = now.AddDays(-3), UpdateTime = now.AddDays(-2) },
            new RepairOrder { UserId = stu4.Id, Category = "水电", Dormitory = "西区2栋106", Description = "宿舍顶灯闪烁，疑似镇流器老化", Status = "done", Priority = "中", AiAdvice = "更换同型号镇流器或整体更换LED灯板。", HandlerId = logistics.Id, CreateTime = now.AddDays(-4), UpdateTime = now.AddDays(-3) },
            new RepairOrder { UserId = stu2.Id, Category = "家具", Dormitory = "西区7栋318", Description = "上床下桌的书桌抽屉滑轨脱落", Status = "pending", Priority = "低", AiAdvice = "重新固定滑轨螺丝，必要时更换滑轨。", CreateTime = now.AddHours(-6) });

        // ---------------- 失物招领（含可匹配演示对） ----------------
        db.LostFounds.AddRange(
            new LostFound { UserId = stu2.Id, Type = "found", Title = "捡到一副黑色蓝牙耳机", Description = "在教一101最后一排桌兜里捡到，黑色入耳式，带充电盒，品牌看不清", Location = "教一101", Contact = "13900000002", CreateTime = now.AddDays(-2) },
            new LostFound { UserId = stu3.Id, Type = "found", Title = "捡到校园卡一张", Description = "卡面姓名：陈xx，在食堂二楼捡到，已交至食堂服务台", Location = "食堂二楼", Contact = "食堂服务台", CreateTime = now.AddDays(-1) },
            new LostFound { UserId = stu1.Id, Type = "lost", Title = "丢失蓝色保温杯", Description = "深蓝色膳魔师保温杯，杯身贴有姓名贴，可能落在图书馆", Location = "图书馆三楼", Contact = "13900000001", CreateTime = now.AddDays(-1) },
            new LostFound { UserId = stu4.Id, Type = "lost", Title = "丢失学生证", Description = "陈晓雯，计算机学院，可能在操场或体育馆丢失", Location = "东区操场", Contact = "13900000004", CreateTime = now.AddHours(-8) },
            new LostFound { UserId = stu2.Id, Type = "found", Title = "捡到一把格子雨伞", Description = "灰蓝色格子折叠伞，在教学楼主入口伞架上", Location = "教一正门", Contact = "13900000002", Status = "closed", CreateTime = now.AddDays(-6) });

        // ---------------- 活动 / 报名 ----------------
        var act1 = new CampusActivity { Title = "校园歌手大赛", Content = "一年一度的校园歌手大赛开始报名！初赛自选曲目清唱90秒，决赛乐队伴奏。设一等奖1名、二等奖2名、三等奖3名，获奖可加综合素质分。", Location = "学术报告厅", StartTime = now.AddDays(10), Quota = 100, CreatorId = admin.Id, CreateTime = now.AddDays(-4) };
        var act2 = new CampusActivity { Title = "编程马拉松 Hackathon", Content = "24小时极限编程挑战，三人一队，主题现场公布。提供夜宵和咖啡，冠军队伍奖金3000元+实习内推机会。", Location = "实验楼", StartTime = now.AddDays(15), Quota = 60, CreatorId = admin.Id, CreateTime = now.AddDays(-3) };
        var act3 = new CampusActivity { Title = "秋季篮球联赛", Content = "以学院为单位组队报名，每队8-12人，小组赛+淘汰赛制。请队长统一报名并提交队员名单。", Location = "东区篮球场", StartTime = now.AddDays(7), Quota = 80, CreatorId = teacher.Id, CreateTime = now.AddDays(-2) };
        var act4 = new CampusActivity { Title = "心理健康讲座：如何应对考前焦虑", Content = "校心理中心专职教师主讲，现场有互动答疑环节，参加可盖讲座章。", Location = "图书馆报告厅", StartTime = now.AddDays(3), Quota = 150, CreatorId = teacher2.Id, CreateTime = now.AddDays(-1) };
        db.CampusActivities.AddRange(act1, act2, act3, act4);
        db.SaveChanges();

        db.ActivitySignups.AddRange(
            new ActivitySignup { ActivityId = act1.Id, UserId = stu2.Id },
            new ActivitySignup { ActivityId = act1.Id, UserId = stu3.Id },
            new ActivitySignup { ActivityId = act2.Id, UserId = stu1.Id },
            new ActivitySignup { ActivityId = act2.Id, UserId = stu4.Id },
            new ActivitySignup { ActivityId = act4.Id, UserId = stu1.Id },
            new ActivitySignup { ActivityId = act4.Id, UserId = stu3.Id, Status = "cancelled" });

        // ---------------- 场地 / 预约 ----------------
        db.VenueResources.AddRange(
            new VenueResource { Name = "学术报告厅", Location = "图书馆一楼", Capacity = 200, Equipment = "投影、音响、舞台灯光" },
            new VenueResource { Name = "篮球场1号场", Location = "东区体育场", Capacity = 30, Equipment = "夜间灯光" },
            new VenueResource { Name = "研讨室A", Location = "图书馆三楼", Capacity = 12, Equipment = "白板、电视投屏" },
            new VenueResource { Name = "多功能厅", Location = "大学生活动中心二楼", Capacity = 80, Equipment = "投影、音响、桌椅可移动" },
            new VenueResource { Name = "琴房3号", Location = "艺术楼一层", Capacity = 4, Equipment = "立式钢琴" });

        db.VenueBookings.AddRange(
            new VenueBooking { VenueId = 1, UserId = stu2.Id, StartTime = now.AddDays(2).Date.AddHours(14), EndTime = now.AddDays(2).Date.AddHours(16), Reason = "社团换届大会", Status = "pending", CreateTime = now.AddHours(-5) },
            new VenueBooking { VenueId = 3, UserId = stu1.Id, StartTime = now.AddDays(1).Date.AddHours(19), EndTime = now.AddDays(1).Date.AddHours(21), Reason = "毕设小组讨论", Status = "approved", CreateTime = now.AddDays(-1) },
            new VenueBooking { VenueId = 2, UserId = stu3.Id, StartTime = now.AddDays(-1).Date.AddHours(18), EndTime = now.AddDays(-1).Date.AddHours(20), Reason = "班级篮球训练", Status = "approved", CreateTime = now.AddDays(-2) },
            new VenueBooking { VenueId = 4, UserId = stu4.Id, StartTime = now.AddDays(3).Date.AddHours(9), EndTime = now.AddDays(3).Date.AddHours(12), Reason = "舞蹈社排练", Status = "rejected", CreateTime = now.AddDays(-1) });

        // ---------------- AI 知识库（12 条） ----------------
        db.AiKnowledges.AddRange(
            new AiKnowledge { Title = "如何申请请假", Category = "请假", Content = "学生请假需登录教务系统提交请假申请，写明请假事由与起止时间，由辅导员审批。3天以上需学院副书记审批，7天以上需教务处备案。" },
            new AiKnowledge { Title = "宿舍报修流程", Category = "报修", Content = "宿舍设施损坏可在本平台移动端提交报修单，注明报修类型、宿舍地址和故障描述，后勤人员接单后上门维修，一般24小时内响应，紧急情况（漏水、漏电）2小时内响应。" },
            new AiKnowledge { Title = "奖学金申请条件", Category = "奖助", Content = "国家奖学金要求学年平均分85分以上且无挂科；励志奖学金面向家庭经济困难学生，平均分80分以上。每年9月开放申请，需在学生工作系统提交材料。" },
            new AiKnowledge { Title = "图书馆借阅规则", Category = "图书馆", Content = "本科生一次最多借阅10册，借期30天，可续借一次15天。逾期未还每册每天罚款0.1元。图书馆开放时间：周一至周日 7:30-22:00。" },
            new AiKnowledge { Title = "校园卡挂失补办", Category = "校园卡", Content = "校园卡丢失后应立即在食堂圈存机或财务处网站挂失，补办需携带学生证到校园卡服务中心（行政楼105），工本费20元，3个工作日后取卡。" },
            new AiKnowledge { Title = "体测免测申请", Category = "体育", Content = "因伤病无法参加体测的学生，需持二甲以上医院证明，填写免测申请表，经体育部审核通过后可免测，成绩按及格计。每学期体测前两周截止申请。" },
            new AiKnowledge { Title = "转专业政策", Category = "教务", Content = "大一学年结束后可申请转专业，要求大一学年平均分75分以上且无挂科。每年6月发布接收计划，需参加接收学院的考核（笔试或面试）。" },
            new AiKnowledge { Title = "勤工助学岗位申请", Category = "奖助", Content = "家庭经济困难学生可申请图书馆助理、实验室助理等勤工助学岗位，时薪15元，每月不超过40小时。在学生资助管理中心网站报名。" },
            new AiKnowledge { Title = "四六级报名与成绩查询", Category = "考试", Content = "四六级每年6月、12月各考一次，报名在教务系统开放，报名费30元。成绩一般在考后两个月公布，可在教务系统或中国教育考试网查询。" },
            new AiKnowledge { Title = "宿舍用电安全规定", Category = "宿舍", Content = "宿舍限电800W，禁止使用热得快、电炉、电饭煲等大功率电器，违者没收电器并通报批评。宿舍无人时请关闭电源，充电器不要长时间插在插座上。" },
            new AiKnowledge { Title = "心理咨询预约", Category = "心理", Content = "校心理健康教育中心提供免费心理咨询，可通过电话（校内分机8888）或心理中心公众号预约，咨询地点：大学生活动中心三楼，全程保密。" },
            new AiKnowledge { Title = "火车票学生优惠卡", Category = "生活", Content = "学生证火车票优惠卡每学年需充值一次（4次单程优惠），充值地点：教务处学籍科。优惠区间仅限学校至家庭所在地，乘车时需携带学生证。" });

        // ---------------- AI 对话记录（历史演示数据） ----------------
        db.AiChatLogs.AddRange(
            new AiChatLog { UserId = stu1.Id, Question = "校园卡丢了怎么办", Answer = "根据校园知识库：校园卡丢失后应立即在食堂圈存机或财务处网站挂失，补办需携带学生证到校园卡服务中心（行政楼105），工本费20元，3个工作日后取卡。", CreateTime = now.AddDays(-2).AddHours(3) },
            new AiChatLog { UserId = stu1.Id, Question = "怎么申请请假", Answer = "学生请假需登录教务系统提交请假申请，写明请假事由与起止时间，由辅导员审批。3天以上需学院副书记审批，7天以上需教务处备案。", CreateTime = now.AddDays(-2).AddHours(4) },
            new AiChatLog { UserId = stu2.Id, Question = "图书馆能借几本书", Answer = "本科生一次最多借阅10册，借期30天，可续借一次15天。逾期未还每册每天罚款0.1元。", CreateTime = now.AddDays(-1).AddHours(2) },
            new AiChatLog { UserId = stu3.Id, Question = "体测受伤了可以免测吗", Answer = "因伤病无法参加体测的学生，需持二甲以上医院证明，填写免测申请表，经体育部审核通过后可免测，成绩按及格计。", CreateTime = now.AddDays(-1).AddHours(6) },
            new AiChatLog { UserId = stu1.Id, Question = "奖学金怎么申请", Answer = "国家奖学金要求学年平均分85分以上且无挂科；励志奖学金面向家庭经济困难学生，平均分80分以上。每年9月开放申请。", CreateTime = now.AddHours(-10) });

        db.SaveChanges();

        // ---------------- 工单进度（接单/完工留痕） ----------------
        db.RepairProgresses.AddRange(
            new RepairProgress { RepairId = 1, Status = "pending", Remark = "学生提交报修，等待后勤接单", OperatorId = stu1.Id, OperatorName = "谭浩杰", CreateTime = now.AddHours(-2) },
            new RepairProgress { RepairId = 2, Status = "pending", Remark = "学生提交报修，等待后勤接单", OperatorId = stu2.Id, OperatorName = "张三", CreateTime = now.AddDays(-1) },
            new RepairProgress { RepairId = 2, Status = "processing", Remark = "已接单，携带网线测试仪上门排查", OperatorId = logistics.Id, OperatorName = "张维修", CreateTime = now.AddHours(-20) },
            new RepairProgress { RepairId = 3, Status = "processing", Remark = "已接单，准备轨道润滑油", OperatorId = logistics.Id, OperatorName = "张维修", CreateTime = now.AddDays(-3).AddHours(4) },
            new RepairProgress { RepairId = 3, Status = "done", Remark = "轨道清理并上油，推拉顺畅，维修完成", OperatorId = logistics.Id, OperatorName = "张维修", CreateTime = now.AddDays(-2) },
            new RepairProgress { RepairId = 4, Status = "done", Remark = "更换LED灯板，通电测试正常", OperatorId = logistics.Id, OperatorName = "张维修", CreateTime = now.AddDays(-3) });

        // ---------------- 公告 ----------------
        db.Announcements.AddRange(
            new Announcement { Title = "关于2025-2026学年第一学期期末考试安排的通知", Category = "teaching", Scope = "all", Status = "published", IsTop = true, PublisherId = admin.Id, PublisherName = "系统管理员", ViewCount = 326, Content = "各位同学：本学期期末考试将于第18-19周进行，具体时间地点请登录教务系统查询。请提前做好复习准备，考试时须携带学生证，严禁携带手机等电子设备进入考场。", CreateTime = now.AddDays(-5) },
            new Announcement { Title = "校园网络设备升级维护公告", Category = "notice", Scope = "all", Status = "published", PublisherId = admin.Id, PublisherName = "系统管理员", ViewCount = 158, Content = "为提升校园网络质量，信息中心将于本周六 00:00-06:00 对西区宿舍楼网络设备进行升级，期间可能短暂断网，请提前保存工作。", CreateTime = now.AddDays(-3) },
            new Announcement { Title = "关于举办第十六届校园文化艺术节的通知", Category = "activity", Scope = "all", Status = "published", PublisherId = teacher.Id, PublisherName = "王建国", ViewCount = 245, Content = "本届校园文化艺术节将于下月开幕，包含歌手大赛、书画展、摄影比赛、话剧专场等活动，欢迎各班级积极报名参与。", CreateTime = now.AddDays(-2) },
            new Announcement { Title = "宿舍冬季用电安全提醒", Category = "life", Scope = "all", Status = "published", PublisherId = admin.Id, PublisherName = "系统管理员", ViewCount = 89, Content = "天气转冷，请同学们规范用电，严禁使用热得快、电炉等大功率电器，离开宿舍请关闭电源，注意防火安全。", CreateTime = now.AddDays(-1) },
            new Announcement { Title = "计算机学院关于组织毕业设计中期检查的通知", Category = "teaching", Scope = "dept", Status = "pending", PublisherId = teacher2.Id, PublisherName = "李慧敏", Content = "请2022级同学于下周五前提交毕业设计中期报告，指导教师审核后统一提交学院。", CreateTime = now.AddHours(-6) });

        // ---------------- 食堂菜单 ----------------
        db.CafeteriaMenus.AddRange(
            new CafeteriaMenu { Canteen = "一食堂", Window = "家常热菜", MealType = "午餐", DishName = "红烧肉盖饭", Price = 14.0m, Description = "肥瘦相间，配青菜" },
            new CafeteriaMenu { Canteen = "一食堂", Window = "家常热菜", MealType = "午餐", DishName = "番茄炒蛋盖饭", Price = 10.0m, Description = "酸甜可口" },
            new CafeteriaMenu { Canteen = "一食堂", Window = "面食窗口", MealType = "午餐", DishName = "牛肉拉面", Price = 12.0m, Description = "现拉现煮" },
            new CafeteriaMenu { Canteen = "一食堂", Window = "早点档", MealType = "早餐", DishName = "小笼包(6个)", Price = 5.0m, Description = "鲜肉小笼包" },
            new CafeteriaMenu { Canteen = "一食堂", Window = "早点档", MealType = "早餐", DishName = "豆浆+油条", Price = 4.0m, Description = "经典搭配" },
            new CafeteriaMenu { Canteen = "二食堂", Window = "麻辣香锅", MealType = "午餐", DishName = "麻辣香锅(自选)", Price = 16.0m, Description = "荤素搭配，可加辣" },
            new CafeteriaMenu { Canteen = "二食堂", Window = "轻食沙拉", MealType = "晚餐", DishName = "鸡胸肉沙拉", Price = 15.0m, Description = "低脂高蛋白" },
            new CafeteriaMenu { Canteen = "二食堂", Window = "特色小吃", MealType = "晚餐", DishName = "酸辣粉", Price = 8.0m, Description = "重庆风味" },
            new CafeteriaMenu { Canteen = "二食堂", Window = "特色小吃", MealType = "晚餐", DishName = "烤冷面", Price = 7.0m, Description = "东北特色" },
            new CafeteriaMenu { Canteen = "民族餐厅", Window = "清真窗口", MealType = "午餐", DishName = "大盘鸡拌面", Price = 18.0m, Description = "分量足" });

        // ---------------- 消息中心 ----------------
        db.Notifications.AddRange(
            new Notification { UserId = stu1.Id, Title = "报修工单已提交", Content = "您的报修「宿舍洗手池水管漏水」已提交，后勤正在安排处理。", Type = "repair", CreateTime = now.AddHours(-2) },
            new Notification { UserId = stu1.Id, Title = "学业预警提醒", Content = "您在《数据结构》的成绩为58分，已触发学业预警，建议尽快联系任课老师。", Type = "ai", IsRead = true, CreateTime = now.AddDays(-1) },
            new Notification { UserId = stu1.Id, Title = "期末考试安排已发布", Content = "本学期期末考试安排已发布，请登录教务系统查询。", Type = "announce", CreateTime = now.AddDays(-5) },
            new Notification { UserId = logistics.Id, Title = "新报修工单", Content = "东区3栋502 提交了新的报修工单（水电类，紧急度：高），请及时处理。", Type = "repair", CreateTime = now.AddHours(-2) },
            new Notification { UserId = logistics.Id, Title = "工单待接单提醒", Content = "当前有 2 条待接单工单，请及时处理。", Type = "repair", IsRead = true, CreateTime = now.AddHours(-6) },
            new Notification { UserId = teacher.Id, Title = "学生咨询待回复", Content = "有新的学生咨询问题待回复。", Type = "system", CreateTime = now.AddHours(-4) },
            new Notification { UserId = admin.Id, Title = "公告待审核", Content = "李慧敏 提交了公告《计算机学院关于组织毕业设计中期检查的通知》，等待审核。", Type = "announce", CreateTime = now.AddHours(-6) });

        // ---------------- 登录/操作日志 ----------------
        db.LoginLogs.AddRange(
            new LoginLog { UserId = admin.Id, Username = "admin", Ip = "127.0.0.1", Result = "success", Message = "登录成功", CreateTime = now.AddHours(-8) },
            new LoginLog { UserId = stu1.Id, Username = "student", Ip = "192.168.1.23", Result = "success", Message = "登录成功", CreateTime = now.AddHours(-6) },
            new LoginLog { Username = "student", Ip = "192.168.1.50", Result = "fail", Message = "密码错误", CreateTime = now.AddHours(-3) },
            new LoginLog { UserId = logistics.Id, Username = "logistics", Ip = "192.168.1.88", Result = "success", Message = "登录成功", CreateTime = now.AddHours(-2) },
            new LoginLog { UserId = teacher.Id, Username = "teacher", Ip = "192.168.1.11", Result = "success", Message = "登录成功", CreateTime = now.AddHours(-1) });

        db.OperationLogs.AddRange(
            new OperationLog { UserId = admin.Id, Username = "admin", Module = "用户管理", Action = "新增用户", Detail = "创建教师账号 teacher2", Ip = "127.0.0.1", CreateTime = now.AddDays(-2) },
            new OperationLog { UserId = logistics.Id, Username = "logistics", Module = "报修工单", Action = "接单", Detail = "接单工单 #2（网络类）", Ip = "192.168.1.88", CreateTime = now.AddHours(-20) },
            new OperationLog { UserId = logistics.Id, Username = "logistics", Module = "报修工单", Action = "完工", Detail = "完成工单 #3（门窗类）", Ip = "192.168.1.88", CreateTime = now.AddDays(-2) },
            new OperationLog { UserId = admin.Id, Username = "admin", Module = "公告管理", Action = "发布公告", Detail = "发布全校公告《期末考试安排》", Ip = "127.0.0.1", CreateTime = now.AddDays(-5) });

        // ---------------- 学生咨询 ----------------
        db.Consultations.AddRange(
            new Consultation { UserId = stu1.Id, Question = "我的毕业设计题目偏难，想换一个方向可以吗？", Category = "学习", CreateTime = now.AddHours(-4) },
            new Consultation { UserId = stu2.Id, Question = "最近总是失眠焦虑，学校有心理咨询吗？", Category = "心理", CreateTime = now.AddDays(-1) },
            new Consultation { UserId = stu3.Id, Question = "宿舍限电800W，我想买个吹风机可以吗？", Category = "生活", Reply = "宿舍可以使用功率不超过800W的吹风机，但请勿同时使用多个大功率电器，注意用电安全。", ReplierId = teacher.Id, ReplierName = "王建国", Status = "replied", CreateTime = now.AddDays(-2), ReplyTime = now.AddDays(-2).AddHours(3) });

        db.SaveChanges();
    }
}
