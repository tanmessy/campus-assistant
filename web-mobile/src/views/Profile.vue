<template>
  <div>
    <van-nav-bar title="个人中心" left-arrow @click-left="$router.back()" />
    <van-cell-group inset style="margin-top:12px">
      <van-cell title="姓名" :value="info.name" />
      <van-cell title="角色" :value="roleLabel" />
      <van-cell title="学号/工号" v-if="info.studentNo" :value="info.studentNo" />
      <van-cell title="用户名" :value="info.username" />
      <van-cell title="手机号">
        <template #value>
          <span>{{ info.phone || '未填写' }}</span>
        </template>
      </van-cell>
    </van-cell-group>

    <template v-if="isStudent">
      <van-cell-group title="我的学业预警" inset style="margin-top:12px">
        <van-cell v-for="a in alerts" :key="a.id" :title="a.courseName" :label="a.reason" @click="markRead(a)">
          <template #value>
            <van-tag :type="a.isRead ? 'default' : 'danger'">{{ a.isRead ? '已读' : '未读' }}</van-tag>
          </template>
        </van-cell>
        <van-empty v-if="!alerts.length" description="暂无预警，继续保持" />
      </van-cell-group>

      <div style="margin:12px 16px">
        <van-button block type="primary" plain icon="magic" :loading="loadingAdvice" @click="getAdvice">AI 学习建议</van-button>
      </div>
      <van-cell-group v-if="advice" title="AI 学习建议" inset>
        <van-cell :label="advice" class="advice" />
      </van-cell-group>
    </template>

    <van-cell-group title="修改密码" inset style="margin-top:12px">
      <van-field v-model="pwd.oldPassword" type="password" label="原密码" placeholder="请输入原密码" />
      <van-field v-model="pwd.newPassword" type="password" label="新密码" placeholder="不少于6位" />
      <van-field v-model="pwd.confirm" type="password" label="确认密码" placeholder="再次输入新密码" />
    </van-cell-group>
    <div style="margin:12px 16px">
      <van-button block type="warning" plain :loading="changing" @click="changePwd">确认修改密码</van-button>
    </div>

    <div style="margin:20px 16px">
      <van-button block type="danger" plain @click="logout">退出登录</van-button>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { showToast } from 'vant'
import api from '../api'

const router = useRouter()
const info = reactive({ name: '', username: '', studentNo: '', phone: '', role: '' })
const alerts = ref([])
const advice = ref('')
const loadingAdvice = ref(false)
const changing = ref(false)
const pwd = reactive({ oldPassword: '', newPassword: '', confirm: '' })

const roleLabel = computed(() => ({ student: '学生', teacher: '教师', logistics: '后勤人员', admin: '管理员' }[info.role] || info.role))
const isStudent = computed(() => info.role === 'student')

async function getAdvice() {
  loadingAdvice.value = true
  try { advice.value = (await api.get('/ai/study-advice')).advice } finally { loadingAdvice.value = false }
}
async function markRead(a) {
  if (a.isRead) return
  await api.post(`/alerts/${a.id}/read`); a.isRead = true
}
async function changePwd() {
  if (!pwd.oldPassword || !pwd.newPassword) return showToast('请填写完整')
  if (pwd.newPassword.length < 6) return showToast('新密码至少6位')
  if (pwd.newPassword !== pwd.confirm) return showToast('两次密码不一致')
  changing.value = true
  try {
    await api.post('/auth/change-password', { oldPassword: pwd.oldPassword, newPassword: pwd.newPassword })
    showToast('密码修改成功，请重新登录')
    setTimeout(() => { localStorage.clear(); router.push('/login') }, 1000)
  } finally { changing.value = false }
}
function logout() { localStorage.clear(); router.push('/login') }

onMounted(async () => {
  Object.assign(info, await api.get('/auth/profile'))
  if (isStudent.value) alerts.value = await api.get('/alerts')
})
</script>

<style scoped>
.advice :deep(.van-cell__label) { white-space: pre-wrap; line-height: 1.8; color: #323233; }
</style>
