<template>
  <div class="login">
    <div class="brand">
      <h2>校园生活智能助手</h2>
      <p>AI 问答 · 课表成绩 · 报修 · 失物招领</p>
    </div>

    <van-form v-if="!isRegister" @submit="doLogin">
      <van-cell-group inset>
        <van-field v-model="form.username" label="用户名" placeholder="请输入用户名" />
        <van-field v-model="form.password" type="password" label="密码" placeholder="请输入密码" />
      </van-cell-group>
      <div style="margin: 24px 16px">
        <van-button round block type="primary" native-type="submit" :loading="loading">登 录</van-button>
        <van-button round block plain type="primary" style="margin-top:12px" @click="isRegister = true">学生注册</van-button>
      </div>
    </van-form>

    <van-form v-else @submit="doRegister">
      <van-cell-group inset>
        <van-field v-model="reg.username" label="用户名" placeholder="3-20位字母/数字/下划线" />
        <van-field v-model="reg.name" label="姓名" placeholder="请输入真实姓名" />
        <van-field v-model="reg.studentNo" label="学号" placeholder="选填" />
        <van-field v-model="reg.phone" label="手机号" placeholder="选填" />
        <van-field v-model="reg.password" type="password" label="密码" placeholder="不少于6位" />
      </van-cell-group>
      <div style="margin: 24px 16px">
        <van-button round block type="success" native-type="submit" :loading="regLoading">注 册</van-button>
        <van-button round block plain style="margin-top:12px" @click="isRegister = false">返回登录</van-button>
      </div>
    </van-form>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { showToast } from 'vant'
import api from '../api'

const router = useRouter()
const form = reactive({ username: '', password: '' })
const reg = reactive({ username: '', name: '', studentNo: '', phone: '', password: '' })
const loading = ref(false)
const regLoading = ref(false)
const isRegister = ref(false)

async function doLogin() {
  loading.value = true
  try {
    const data = await api.post('/auth/login', form)
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(data.user))
    router.push('/')
  } finally { loading.value = false }
}

async function doRegister() {
  if (!reg.username || reg.username.length < 3) return showToast('用户名至少3位')
  if (!reg.name) return showToast('请填写姓名')
  if (!reg.password || reg.password.length < 6) return showToast('密码至少6位')
  regLoading.value = true
  try {
    await api.post('/auth/register', {
      username: reg.username, password: reg.password, name: reg.name,
      studentNo: reg.studentNo || null, phone: reg.phone || null
    })
    showToast('注册成功，请登录')
    form.username = reg.username
    isRegister.value = false
  } finally { regLoading.value = false }
}
</script>

<style scoped>
.login { padding-top: 70px; }
.brand { text-align: center; margin-bottom: 30px; }
.brand h2 { margin: 0; }
.brand p { color: #969799; font-size: 13px; }
.tip { text-align: center; color: #c8c9cc; font-size: 12px; }
</style>
