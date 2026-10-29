# 校园生活智能助手平台

基于 ASP.NET Core 8 + Vue 3 的高校校园生活综合服务平台，整合报修、失物招领、通知公告、AI 问答等日常服务，并接入大语言模型提供智能问答与报修智能分类能力。

## 技术栈

| 层 | 技术 |
|---|---|
| 后端 | ASP.NET Core 8、Entity Framework Core 8、MySQL 8、JWT 认证 |
| PC 前端 | Vue 3、Vite、Element Plus、Pinia |
| 移动前端 | Vue 3、Vite、Vant |
| AI 能力 | 智谱 GLM-4-Flash（OpenAI 兼容协议）、Embedding 向量检索、RAG |
| 接口风格 | RESTful API |

## 功能模块

- **学生端**：宿舍报修（AI 智能分类派单）、失物招领（AI 语义匹配）、校园公告、课表查询、食堂菜单、校园活动、场地预约、AI 智能问答、学业预警
- **后勤端**：报修工单接单、派单、进度更新、维修照片上传、绩效统计
- **教师端**：课务成绩录入、公告发布、学生咨询回复
- **管理员端**：用户管理、公告审核、知识库维护、操作日志、平台数据看板

## 项目结构

```
bysj/
├── server/                  # ASP.NET Core 后端
│   └── CampusAssistant.Api/
│       ├── Controllers/     # API 控制器
│       ├── Data/            # DbContext
│       ├── Models/          # 实体与 DTO
│       ├── Services/        # AI 服务、业务服务
│       └── appsettings.json  # 配置（敏感项已用占位符）
├── web-admin/               # PC 管理端（Vue3 + Element Plus）
├── web-mobile/              # 移动端 H5（Vue3 + Vant）
└── campus_assistant.sql     # 数据库初始化脚本
```

## 快速启动

### 环境要求
- .NET 8 SDK
- Node.js 18+
- MySQL 8.0

### 步骤

1. **初始化数据库**
   ```bash
   mysql -u root -p < campus_assistant.sql
   ```

2. **配置后端**
   修改 `server/CampusAssistant.Api/appsettings.json`：
   - 数据库连接字符串改为你本地的 MySQL 配置
   - AI 部分填入你自己的智谱 API Key（在 [智谱开放平台](https://open.bigmodel.cn) 申请）

3. **启动后端**
   ```bash
   cd server/CampusAssistant.Api
   dotnet run
   # 默认运行在 http://localhost:5000
   ```

4. **启动 PC 端**
   ```bash
   cd web-admin
   npm install
   npm run dev
   # 默认运行在 http://localhost:5173
   ```

5. **启动移动端**
   ```bash
   cd web-mobile
   npm install
   npm run dev
   # 默认运行在 http://localhost:5174
   ```

## 测试账号

| 角色 | 账号 | 密码 |
|---|---|---|
| 管理员 | admin | 123456 |
| 教师 | teacher | 123456 |
| 后勤 | log | 123456 |
| 学生 | student | 123456 |

## 核心亮点

- **RAG 校园智能问答**：基于知识库的检索增强生成，支持"即录即答"，提示词约束防止幻觉
- **报修 AI 智能分类**：学生用自然语言描述故障，后端自动分类、评估紧急度并生成维修建议
- **失物招领语义匹配**：基于向量相似度，能匹配字面不同但语义相近的寻物/招领信息
- **多角色权限控制**：JWT 认证 + 前端路由守卫 + 后端角色特性校验，双层防护

## 项目截图

> （此处可补充系统运行截图：登录页、报修流程、AI 问答、管理端看板）

## 作者

谭浩杰
