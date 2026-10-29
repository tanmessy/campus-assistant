<template>
  <el-container style="height:100vh">
    <el-aside width="220px" class="aside">
      <div class="logo">校园生活智能助手</div>
      <el-menu :default-active="$route.path" router background-color="#001529" text-color="#a6adb4" active-text-color="#fff">
        <el-menu-item v-for="m in menus" :key="m.path" :index="m.path">
          <el-icon><component :is="m.icon" /></el-icon><span>{{ m.title }}</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    <el-container>
      <el-header class="header">
        <span class="title">{{ $route.meta.title }}</span>
        <div class="right">
          <el-badge :value="unread" :hidden="unread === 0" class="bell">
            <el-icon style="cursor:pointer;font-size:19px" @click="$router.push('/messages')"><Bell /></el-icon>
          </el-badge>
          <el-dropdown @command="onCmd">
            <span class="user">{{ user?.name }}（{{ roleLabel }}）<el-icon><ArrowDown /></el-icon></span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">个人信息</el-dropdown-item>
                <el-dropdown-item command="logout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>
      <el-main style="background:#f5f7fa"><router-view /></el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  DataBoard, User, Tools, Reading, Calendar, ChatDotRound, Bell, ArrowDown,
  Document, DataLine, Setting, Memo, ChatLineSquare, List, Message
} from '@element-plus/icons-vue'
import api from '../api'
import { currentUser, roleNameMap } from '../router'

const router = useRouter()
const user = currentUser()
const role = user?.role || ''
const roleLabel = computed(() => roleNameMap[role] || role)
const unread = ref(0)

// 角色 → 可见菜单（前端隐藏无权限菜单）
const ALL_MENUS = [
  { path: '/dashboard', title: '首页仪表盘', icon: DataBoard, roles: ['admin'] },
  { path: '/users', title: '用户管理', icon: User, roles: ['admin'] },
  { path: '/announcement-audit', title: '公告审核', icon: Document, roles: ['admin'] },
  { path: '/knowledge', title: '知识库与对话', icon: ChatDotRound, roles: ['admin'] },
  { path: '/settings', title: '基础配置', icon: Setting, roles: ['admin'] },
  { path: '/logs', title: '日志管理', icon: List, roles: ['admin'] },
  { path: '/repairs', title: '报修工单', icon: Tools, roles: ['logistics', 'admin'] },
  { path: '/logistics-stats', title: '报修数据统计', icon: DataLine, roles: ['logistics', 'admin'] },
  { path: '/announcements-manage', title: '公告管理', icon: Document, roles: ['teacher', 'admin'] },
  { path: '/consultations', title: '学生咨询', icon: ChatLineSquare, roles: ['teacher', 'admin'] },
  { path: '/courses', title: '课务与学生', icon: Reading, roles: ['teacher', 'admin'] },
  { path: '/activities', title: '活动与场地', icon: Calendar, roles: ['teacher', 'admin'] },
  { path: '/my-schedule', title: '我的课表', icon: Memo, roles: ['student'] },
  { path: '/announcements', title: '校园公告', icon: Document, roles: ['student', 'teacher'] },
  { path: '/my-repairs', title: '我的报修', icon: Tools, roles: ['student'] },
  { path: '/ai-web', title: 'AI 智能助手', icon: ChatDotRound, roles: ['student'] },
  { path: '/messages', title: '消息中心', icon: Message, roles: ['admin', 'teacher', 'logistics', 'student'] },
  { path: '/profile', title: '个人信息', icon: User, roles: ['admin', 'teacher', 'logistics', 'student'] }
]
const menus = ALL_MENUS.filter(m => m.roles.includes(role))

async function loadUnread() {
  try { const d = await api.get('/notifications/unread-count'); unread.value = d.count } catch { /* ignore */ }
}
onMounted(loadUnread)

function onCmd(cmd) {
  if (cmd === 'logout') { localStorage.clear(); router.push('/login') }
  if (cmd === 'profile') router.push('/profile')
}
</script>

<style scoped>
.aside { background: #001529; overflow-y: auto; }
.logo { color: #fff; font-size: 16px; font-weight: 500; text-align: center; padding: 18px 8px; letter-spacing: 1px; }
.aside :deep(.el-menu) { border-right: none; }
.header { display: flex; align-items: center; justify-content: space-between; background: #fff; border-bottom: 1px solid #eee; }
.title { font-size: 16px; font-weight: 500; }
.right { display: flex; align-items: center; gap: 22px; }
.bell { display: flex; align-items: center; }
.user { cursor: pointer; color: #606266; display: flex; align-items: center; gap: 4px; }
</style>
