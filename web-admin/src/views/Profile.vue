<template>
  <el-row :gutter="16">
    <el-col :span="12">
      <el-card shadow="never">
        <template #header>个人信息</template>
        <el-form :model="form" label-width="90px">
          <el-form-item label="用户名"><el-input v-model="form.username" disabled /></el-form-item>
          <el-form-item label="角色">
            <el-tag>{{ roleLabel }}</el-tag>
          </el-form-item>
          <el-form-item label="学号" v-if="form.studentNo"><el-input v-model="form.studentNo" disabled /></el-form-item>
          <el-form-item label="姓名"><el-input v-model="form.name" /></el-form-item>
          <el-form-item label="手机号"><el-input v-model="form.phone" /></el-form-item>
          <el-form-item label="邮箱"><el-input v-model="form.email" /></el-form-item>
          <el-button type="primary" :loading="saving" @click="save">保存资料</el-button>
        </el-form>
      </el-card>
    </el-col>
    <el-col :span="12">
      <el-card shadow="never">
        <template #header>修改密码</template>
        <el-form :model="pwd" label-width="90px">
          <el-form-item label="原密码"><el-input v-model="pwd.oldPassword" type="password" show-password /></el-form-item>
          <el-form-item label="新密码"><el-input v-model="pwd.newPassword" type="password" show-password placeholder="不少于6位" /></el-form-item>
          <el-form-item label="确认密码"><el-input v-model="pwd.confirm" type="password" show-password /></el-form-item>
          <el-button type="warning" :loading="changing" @click="changePwd">修改密码</el-button>
        </el-form>
      </el-card>
    </el-col>
  </el-row>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import api from '../api'
import { roleNameMap } from '../router'

const form = reactive({ username: '', name: '', phone: '', email: '', studentNo: '', role: '' })
const pwd = reactive({ oldPassword: '', newPassword: '', confirm: '' })
const saving = ref(false)
const changing = ref(false)
const roleLabel = computed(() => roleNameMap[form.role] || form.role)

async function load() {
  const d = await api.get('/auth/profile')
  Object.assign(form, { username: d.username, name: d.name, phone: d.phone, email: d.email, studentNo: d.studentNo, role: d.role })
}
async function save() {
  if (!form.name) return ElMessage.warning('姓名不能为空')
  saving.value = true
  try {
    await api.post('/auth/profile', { name: form.name, phone: form.phone, email: form.email })
    ElMessage.success('资料已更新')
    const u = JSON.parse(localStorage.getItem('user') || '{}')
    u.name = form.name
    localStorage.setItem('user', JSON.stringify(u))
    load()
  } finally { saving.value = false }
}
async function changePwd() {
  if (!pwd.oldPassword || !pwd.newPassword) return ElMessage.warning('请填写完整')
  if (pwd.newPassword.length < 6) return ElMessage.warning('新密码至少6位')
  if (pwd.newPassword !== pwd.confirm) return ElMessage.warning('两次密码不一致')
  changing.value = true
  try {
    await api.post('/auth/change-password', { oldPassword: pwd.oldPassword, newPassword: pwd.newPassword })
    ElMessage.success('密码修改成功，请重新登录')
    setTimeout(() => { localStorage.clear(); location.href = '/login' }, 1000)
  } finally { changing.value = false }
}
onMounted(load)
</script>
