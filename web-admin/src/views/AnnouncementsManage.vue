<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>公告管理</span>
        <el-button type="primary" :icon="Plus" @click="openEdit()">发布公告</el-button>
      </div>
    </template>

    <el-table :data="list" stripe v-loading="loading">
      <el-table-column prop="title" label="标题" min-width="220" show-overflow-tooltip />
      <el-table-column label="分类" width="90">
        <template #default="{ row }">{{ catName(row.category) }}</template>
      </el-table-column>
      <el-table-column label="范围" width="90">
        <template #default="{ row }">{{ row.scope === 'all' ? '全校' : row.scope === 'dept' ? '院系' : '班级' }}</template>
      </el-table-column>
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="stType(row.status)" size="small">{{ stName(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="viewCount" label="浏览" width="80" />
      <el-table-column label="发布时间" width="160">
        <template #default="{ row }">{{ fmt(row.createTime) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="150">
        <template #default="{ row }">
          <el-button link type="primary" @click="openEdit(row)">编辑</el-button>
          <el-button link type="danger" @click="del(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="!loading && !list.length" description="暂无公告" />

    <el-dialog v-model="dialog" :title="form.id ? '编辑公告' : '发布公告'" width="640px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="分类">
          <el-select v-model="form.category">
            <el-option label="通知" value="notice" />
            <el-option label="教务" value="teaching" />
            <el-option label="生活" value="life" />
            <el-option label="活动" value="activity" />
          </el-select>
        </el-form-item>
        <el-form-item label="范围">
          <el-select v-model="form.scope">
            <el-option label="全校" value="all" />
            <el-option label="院系" value="dept" />
            <el-option label="班级" value="class" />
          </el-select>
        </el-form-item>
        <el-form-item label="置顶"><el-switch v-model="form.isTop" /></el-form-item>
        <el-form-item label="内容"><el-input v-model="form.content" type="textarea" :rows="7" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="save">提交</el-button>
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
const form = reactive({ id: null, title: '', content: '', category: 'notice', scope: 'all', status: 'pending', isTop: false })

const catName = c => ({ notice: '通知', teaching: '教务', life: '生活', activity: '活动' }[c] || c)
const stName = s => ({ pending: '待审核', published: '已发布', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'warning', published: 'success', rejected: 'danger' }[s] || 'info')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try { list.value = await api.get('/announcements/manage') } finally { loading.value = false }
}
function openEdit(row) {
  if (row) Object.assign(form, { id: row.id, title: row.title, content: row.content, category: row.category, scope: row.scope, status: row.status, isTop: row.isTop })
  else Object.assign(form, { id: null, title: '', content: '', category: 'notice', scope: 'all', status: 'pending', isTop: false })
  dialog.value = true
}
async function save() {
  if (!form.title.trim() || !form.content.trim()) return ElMessage.warning('请填写标题和内容')
  saving.value = true
  try {
    const body = { title: form.title, content: form.content, category: form.category, scope: form.scope, status: form.status, isTop: form.isTop }
    if (form.id) await api.put(`/announcements/${form.id}`, body)
    else await api.post('/announcements', body)
    ElMessage.success('已提交（教师发布需管理员审核）')
    dialog.value = false
    load()
  } finally { saving.value = false }
}
async function del(row) {
  await ElMessageBox.confirm(`确定删除公告「${row.title}」？`, '提示', { type: 'warning' })
  await api.delete(`/announcements/${row.id}`)
  ElMessage.success('已删除')
  load()
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
</style>
