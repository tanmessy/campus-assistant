<template>
  <div class="login-page">
    <el-card class="login-card">
      <h2>校园生活智能助手平台</h2>
      <p class="sub">PC 端 · 学生 / 教师 / 后勤 / 管理员</p>

      <el-tabs v-model="tab" stretch>
        <el-tab-pane label="登录" name="login">
          <el-form :model="form" @keyup.enter="doLogin">
            <el-form-item>
              <el-input v-model="form.username" placeholder="用户名" size="large" :prefix-icon="User" />
            </el-form-item>
            <el-form-item>
              <el-input v-model="form.password" type="password" placeholder="密码" size="large" show-password :prefix-icon="Lock" />
            </el-form-item>
            <el-button type="primary" size="large" style="width:100%" :loading="loading" @click="doLogin">登 录</el-button>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="学生注册" name="register">
          <el-form :model="reg" @keyup.enter="doRegister">
            <el-form-item><el-input v-model="reg.username" placeholder="用户名（3-20位字母/数字/下划线）" size="large" /></el-form-item>
            <el-form-item><el-input v-model="reg.name" placeholder="真实姓名" size="large" /></el-form-item>
            <el-form-item><el-input v-model="reg.studentNo" placeholder="学号（选填）" size="large" /></el-form-item>
            <el-form-item><el-input v-model="reg.phone" placeholder="手机号（选填）" size="large" /></el-form-item>
            <el-form-item><el-input v-model="reg.password" type="password" placeholder="密码（不少于6位）" size="large" show-password /></el-form-item>
            <el-form-item><el-input v-model="reg.password2" type="password" placeholder="确认密码" size="large" show-password /></el-form-item>
            <el-button type="success" size="large" style="width:100%" :loading="regLoading" @click="doRegister">注 册</el-button>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'
import api from '../api'
import { roleHome } from '../router'

const router = useRouter()
const tab = ref('login')
const form = reactive({ username: '', password: '' })
const reg = reactive({ username: '', password: '', password2: '', name: '', studentNo: '', phone: '' })
const loading = ref(false)
const regLoading = ref(false)

async function doLogin() {
  if (!form.username || !form.password) return ElMessage.warning('请输入用户名和密码')
  loading.value = true
  try {
    const data = await api.post('/auth/login', form)
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(data.user))
    ElMessage.success('登录成功')
    router.push(roleHome[data.user.role] || '/profile')
  } finally { loading.value = false }
}

async function doRegister() {
  if (!reg.username || reg.username.length < 3) return ElMessage.warning('用户名至少 3 位')
  if (!reg.name) return ElMessage.warning('请填写真实姓名')
  if (!reg.password || reg.password.length < 6) return ElMessage.warning('密码至少 6 位')
  if (reg.password !== reg.password2) return ElMessage.warning('两次输入的密码不一致')
  regLoading.value = true
  try {
    await api.post('/auth/register', {
      username: reg.username, password: reg.password, name: reg.name,
      studentNo: reg.studentNo || null, phone: reg.phone || null
    })
    ElMessage.success('注册成功，请登录')
    form.username = reg.username
    tab.value = 'login'
  } finally { regLoading.value = false }
}
</script>

<style scoped>
.login-page { height: 100vh; display: flex; align-items: center; justify-content: center; background: #f0f4fa; }
.login-card { width: 420px; text-align: center; }
h2 { margin: 8px 0 0; }
.sub { color: #909399; margin: 4px 0 16px; font-size: 13px; }
.tip { color: #c0c4cc; font-size: 12px; margin-top: 16px; }
</style>
