<template>
  <div>
    <van-nav-bar :title="`校园智能助手 · ${roleLabel}`" />
    <van-notice-bar v-if="unread" left-icon="volume-o" :text="`你有 ${unread} 条未读消息，点击查看`" @click="$router.push('/messages')" />

    <!-- 学生 -->
    <template v-if="role === 'student'">
      <van-notice-bar v-if="unreadAlerts > 0" left-icon="warning-o" :text="`你有 ${unreadAlerts} 条学业预警，点击查看`" @click="$router.push('/profile')" />
      <van-grid :column-num="3" :border="false" style="margin-top:8px">
        <van-grid-item icon="chat-o" text="AI 问答" to="/ai" />
        <van-grid-item icon="todo-list-o" text="课表成绩" to="/schedule" />
        <van-grid-item icon="setting-o" text="宿舍报修" to="/repair" />
        <van-grid-item icon="search" text="失物招领" to="/lostfound" />
        <van-grid-item icon="flag-o" text="校园活动" to="/activities" />
        <van-grid-item icon="location-o" text="场地预约" to="/venue" />
        <van-grid-item icon="orders-o" text="校园公告" to="/announcements" />
        <van-grid-item icon="shop-o" text="食堂菜单" to="/cafeteria" />
        <van-grid-item icon="manager-o" text="个人中心" to="/profile" />
      </van-grid>
    </template>

    <!-- 教师 -->
    <template v-else-if="role === 'teacher'">
      <van-grid :column-num="2" :border="false" style="margin-top:8px">
        <van-grid-item icon="records" text="我的公告" to="/t-announcements" />
        <van-grid-item icon="comment-o" text="学生咨询" to="/t-consultations" />
        <van-grid-item icon="bullhorn-o" text="校园公告" to="/announcements" />
        <van-grid-item icon="manager-o" text="个人中心" to="/profile" />
      </van-grid>
      <van-notice-bar style="margin:10px 0" wrapable :scrollable="false"
        text="提示：教师端移动版仅保留高频简单功能，发布/编辑公告请在 PC 端操作。" />
    </template>

    <!-- 后勤 -->
    <template v-else-if="role === 'logistics'">
      <van-grid :column-num="2" :border="false" style="margin-top:8px">
        <van-grid-item icon="todo-list-o" text="报修工单" to="/l-orders" />
        <van-grid-item icon="comment-o" text="消息中心" to="/messages" />
        <van-grid-item icon="manager-o" text="个人中心" to="/profile" />
      </van-grid>
      <van-notice-bar style="margin:10px 0" wrapable :scrollable="false"
        text="提示：外勤现场可用手机接单、更新进度、上传现场照片、完工确认。" />
    </template>

    <!-- 管理员（极简） -->
    <template v-else>
      <van-grid :column-num="2" :border="false" style="margin-top:8px">
        <van-grid-item icon="comment-o" text="消息中心" to="/messages" />
        <van-grid-item icon="manager-o" text="个人中心" to="/profile" />
      </van-grid>
      <van-notice-bar style="margin:10px 0" wrapable :scrollable="false"
        text="提示：管理员移动端仅支持应急查看（消息、平台概览），复杂管理请使用 PC 后台。" />
      <van-cell-group inset title="平台概览">
        <van-cell title="用户数" :value="String(overview.userCount ?? '-')" />
        <van-cell title="待处理工单" :value="String(overview.pendingRepairs ?? '-')" />
        <van-cell title="今日登录" :value="String(overview.todayLogins ?? '-')" />
      </van-cell-group>
    </template>

    <van-cell-group v-if="role === 'student'" title="最新活动" inset style="margin-top:12px">
      <van-cell v-for="a in activities.slice(0, 3)" :key="a.id" :title="a.title" :label="a.location" is-link to="/activities" />
    </van-cell-group>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import api from '../api'

const activities = ref([])
const unreadAlerts = ref(0)
const unread = ref(0)
const overview = ref({})
const role = computed(() => {
  try { return JSON.parse(localStorage.getItem('user') || '{}').role || 'student' } catch { return 'student' }
})
const roleLabel = computed(() => ({ student: '学生', teacher: '教师', logistics: '后勤', admin: '管理员' }[role.value] || '学生'))

onMounted(async () => {
  const d = await api.get('/notifications/unread-count'); unread.value = d.count
  if (role.value === 'student') {
    activities.value = await api.get('/activities')
    const alerts = await api.get('/alerts')
    unreadAlerts.value = alerts.filter(a => !a.isRead).length
  }
  if (role.value === 'admin') overview.value = await api.get('/stats/dashboard')
})
</script>
