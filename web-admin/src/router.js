import { createRouter, createWebHistory } from 'vue-router'

// 各角色的默认首页
export const roleHome = {
  admin: '/dashboard',
  teacher: '/announcements-manage',
  logistics: '/repairs',
  student: '/my-schedule'
}

// 角色中文名
export const roleNameMap = { admin: '系统管理员', teacher: '教师', logistics: '后勤人员', student: '学生' }

const routes = [
  { path: '/login', component: () => import('./views/Login.vue') },
  {
    path: '/',
    component: () => import('./views/Layout.vue'),
    redirect: '/home',
    children: [
      // ---- 学生端（PC 偏查询/导出/长文本） ----
      { path: 'my-schedule', component: () => import('./views/MySchedule.vue'), meta: { title: '我的课表', roles: ['student'] } },
      { path: 'announcements', component: () => import('./views/Announcements.vue'), meta: { title: '校园公告', roles: ['student', 'teacher'] } },
      { path: 'my-repairs', component: () => import('./views/MyRepairs.vue'), meta: { title: '我的报修', roles: ['student'] } },
      { path: 'ai-web', component: () => import('./views/AiWeb.vue'), meta: { title: 'AI 智能助手', roles: ['student'] } },

      // ---- 教师端 ----
      { path: 'announcements-manage', component: () => import('./views/AnnouncementsManage.vue'), meta: { title: '公告管理', roles: ['teacher', 'admin'] } },
      { path: 'consultations', component: () => import('./views/Consultations.vue'), meta: { title: '学生咨询', roles: ['teacher', 'admin'] } },
      { path: 'courses', component: () => import('./views/Courses.vue'), meta: { title: '课务与学生', roles: ['teacher', 'admin'] } },
      { path: 'activities', component: () => import('./views/Activities.vue'), meta: { title: '活动与场地', roles: ['teacher', 'admin'] } },

      // ---- 后勤端 ----
      { path: 'repairs', component: () => import('./views/Repairs.vue'), meta: { title: '报修工单', roles: ['logistics', 'admin'] } },
      { path: 'logistics-stats', component: () => import('./views/LogisticsStats.vue'), meta: { title: '报修数据统计', roles: ['logistics', 'admin'] } },

      // ---- 管理员端 ----
      { path: 'dashboard', component: () => import('./views/Dashboard.vue'), meta: { title: '首页仪表盘', roles: ['admin'] } },
      { path: 'users', component: () => import('./views/Users.vue'), meta: { title: '用户管理', roles: ['admin'] } },
      { path: 'announcement-audit', component: () => import('./views/AnnouncementAudit.vue'), meta: { title: '公告审核', roles: ['admin'] } },
      { path: 'knowledge', component: () => import('./views/Knowledge.vue'), meta: { title: '知识库与对话', roles: ['admin'] } },
      { path: 'settings', component: () => import('./views/Settings.vue'), meta: { title: '基础配置', roles: ['admin'] } },
      { path: 'logs', component: () => import('./views/Logs.vue'), meta: { title: '日志管理', roles: ['admin'] } },

      // ---- 通用 ----
      { path: 'messages', component: () => import('./views/Messages.vue'), meta: { title: '消息中心' } },
      { path: 'profile', component: () => import('./views/Profile.vue'), meta: { title: '个人信息' } }
    ]
  }
]

const router = createRouter({ history: createWebHistory(), routes })

export function currentUser() {
  try { return JSON.parse(localStorage.getItem('user') || 'null') } catch { return null }
}

router.beforeEach(to => {
  if (to.path === '/login') return true
  if (!localStorage.getItem('token')) return '/login'
  // 访问根路径 → 按角色跳默认首页
  if (to.path === '/home') return roleHome[currentUser()?.role] || '/profile'
  // 角色权限校验：越权访问重定向到本角色首页
  const roles = to.meta?.roles
  if (roles && !roles.includes(currentUser()?.role)) {
    return roleHome[currentUser()?.role] || '/profile'
  }
  return true
})

export default router
