import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  { path: '/login', component: () => import('./views/Login.vue'), meta: { title: '登录' } },
  { path: '/', component: () => import('./views/Home.vue'), meta: { title: '校园智能助手' } },

  // ---- 学生端 ----
  { path: '/ai', component: () => import('./views/AiChat.vue'), meta: { title: 'AI 问答', roles: ['student'] } },
  { path: '/schedule', component: () => import('./views/Schedule.vue'), meta: { title: '课表与成绩', roles: ['student'] } },
  { path: '/repair', component: () => import('./views/Repair.vue'), meta: { title: '宿舍报修', roles: ['student'] } },
  { path: '/lostfound', component: () => import('./views/LostFound.vue'), meta: { title: '失物招领', roles: ['student'] } },
  { path: '/activities', component: () => import('./views/Activities.vue'), meta: { title: '校园活动', roles: ['student'] } },
  { path: '/venue', component: () => import('./views/Venue.vue'), meta: { title: '场地预约', roles: ['student'] } },
  { path: '/announcements', component: () => import('./views/Announcements.vue'), meta: { title: '校园公告' } },
  { path: '/cafeteria', component: () => import('./views/Cafeteria.vue'), meta: { title: '食堂菜单', roles: ['student'] } },

  // ---- 教师端（轻量化） ----
  { path: '/t-announcements', component: () => import('./views/teacher/MyAnnouncements.vue'), meta: { title: '我的公告', roles: ['teacher'] } },
  { path: '/t-consultations', component: () => import('./views/teacher/Consultations.vue'), meta: { title: '学生咨询', roles: ['teacher'] } },

  // ---- 后勤端（外勤） ----
  { path: '/l-orders', component: () => import('./views/logistics/WorkOrders.vue'), meta: { title: '报修工单', roles: ['logistics'] } },
  { path: '/l-order/:id', component: () => import('./views/logistics/OrderDetail.vue'), meta: { title: '工单详情', roles: ['logistics'] } },

  // ---- 通用 ----
  { path: '/messages', component: () => import('./views/Messages.vue'), meta: { title: '消息中心' } },
  { path: '/profile', component: () => import('./views/Profile.vue'), meta: { title: '个人中心' } }
]

const router = createRouter({ history: createWebHistory(), routes })

function user() {
  try { return JSON.parse(localStorage.getItem('user') || 'null') } catch { return null }
}

router.beforeEach(to => {
  document.title = to.meta.title || '校园智能助手'
  if (to.path === '/login') return true
  if (!localStorage.getItem('token')) return '/login'
  const roles = to.meta?.roles
  if (roles && !roles.includes(user()?.role)) return '/'
  return true
})

export default router
