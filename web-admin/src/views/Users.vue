<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>用户管理</span>
        <div>
          <el-select v-model="role" placeholder="全部角色" clearable style="width:130px;margin-right:10px" @change="reload">
            <el-option label="学生" value="student" />
            <el-option label="教师" value="teacher" />
            <el-option label="后勤" value="logistics" />
            <el-option label="管理员" value="admin" />
          </el-select>
          <el-input v-model="keyword" placeholder="搜索用户名/姓名" style="width:180px;margin-right:10px" clearable @keyup.enter="reload" />
          <el-button type="primary" :icon="Plus" @click="openCreate">新增用户</el-button>
        </div>
      </div>
    </template>

    <el-table :data="list" v-loading="loading" stripe>
      <el-table-column prop="username" label="用户名" width="120" />
      <el-table-column prop="name" label="姓名" width="110" />
      <el-table-column label="角色" width="100">
        <template #default="{ row }">
          <el-tag :type="{ admin: 'danger', teacher: 'primary', logistics: 'warning', student: 'success' }[row.role]">
            {{ roleName(row.role) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="studentNo" label="学号/工号" width="140" />
      <el-table-column prop="phone" label="手机号" width="130" />
      <el-table-column label="状态" width="90">
        <template #default="{ row }">
          <el-tag :type="row.status === 'disabled' ? 'info' : 'success'" size="small">
            {{ row.status === 'disabled' ? '已禁用' : '正常' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="240" fixed="right">
        <template #default="{ row }">
          <el-button size="small" link type="primary" @click="resetPwd(row)">重置密码</el-button>
          <el-button v-if="row.role !== 'admin'" size="small" link :type="row.status === 'disabled' ? 'success' : 'warning'" @click="toggle(row)">
            {{ row.status === 'disabled' ? '启用' : '禁用' }}
          </el-button>
          <el-button size="small" link type="danger" @click="del(row)" :disabled="row.role === 'admin'">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination style="margin-top:16px;justify-content:flex-end" layout="total, sizes, prev, pager, next"
      :total="total" :page-size="pageSize" :current-page="pageIndex" :page-sizes="[10, 20, 50]"
      @current-change="onPage" @size-change="onSize" />

    <el-dialog v-model="dialog" title="新增用户" width="440px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="用户名"><el-input v-model="form.username" /></el-form-item>
        <el-form-item label="姓名"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="角色">
          <el-select v-model="form.role" style="width:100%">
            <el-option label="学生" value="student" /><el-option label="教师" value="teacher" />
            <el-option label="后勤" value="logistics" /><el-option label="管理员" value="admin" />
          </el-select>
        </el-form-item>
        <el-form-item label="学号/工号"><el-input v-model="form.studentNo" /></el-form-item>
        <el-form-item label="手机号"><el-input v-model="form.phone" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="create">确定（初始密码 123456）</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import api from '../api'

const list = ref([])
const loading = ref(false)
const dialog = ref(false)
const saving = ref(false)
const role = ref('')
const keyword = ref('')
const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const form = reactive({ username: '', name: '', role: 'student', studentNo: '', phone: '' })

const roleName = r => ({ admin: '管理员', teacher: '教师', logistics: '后勤', student: '学生' }[r] || r)

async function load() {
  loading.value = true
  try {
    const d = await api.get('/users/page', { params: { pageIndex: pageIndex.value, pageSize: pageSize.value, role: role.value || undefined, keyword: keyword.value || undefined } })
    list.value = d.list; total.value = d.total
  } finally { loading.value = false }
}
function reload() { pageIndex.value = 1; load() }
function onPage(p) { pageIndex.value = p; load() }
function onSize(s) { pageSize.value = s; pageIndex.value = 1; load() }

function openCreate() {
  Object.assign(form, { username: '', name: '', role: 'student', studentNo: '', phone: '' })
  dialog.value = true
}
async function create() {
  if (!form.username || !form.name) return ElMessage.warning('请填写用户名和姓名')
  saving.value = true
  try {
    await api.post('/users', form)
    ElMessage.success('创建成功，初始密码 123456')
    dialog.value = false; load()
  } finally { saving.value = false }
}
async function resetPwd(row) {
  await api.post(`/users/${row.id}/reset-password`)
  ElMessage.success(`已将 ${row.name} 的密码重置为 123456`)
}
async function toggle(row) {
  const next = row.status === 'disabled' ? 'enabled' : 'disabled'
  await api.post(`/users/${row.id}/status`, { status: next })
  ElMessage.success(next === 'disabled' ? '已禁用' : '已启用')
  load()
}
async function del(row) {
  await ElMessageBox.confirm(`确定删除用户 ${row.name}？`, '提示', { type: 'warning' })
  await api.delete(`/users/${row.id}`)
  ElMessage.success('已删除'); load()
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
</style>
