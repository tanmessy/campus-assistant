<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>公告审核</span>
        <el-radio-group v-model="tab" @change="load">
          <el-radio-button label="pending">待审核</el-radio-button>
          <el-radio-button label="published">已发布</el-radio-button>
          <el-radio-button label="rejected">已驳回</el-radio-button>
          <el-radio-button label="">全部</el-radio-button>
        </el-radio-group>
      </div>
    </template>

    <el-table :data="list" stripe v-loading="loading">
      <el-table-column prop="title" label="标题" min-width="220" show-overflow-tooltip />
      <el-table-column prop="publisherName" label="发布人" width="110" />
      <el-table-column label="分类" width="90">
        <template #default="{ row }">{{ catName(row.category) }}</template>
      </el-table-column>
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="stType(row.status)" size="small">{{ stName(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="提交时间" width="160">
        <template #default="{ row }">{{ fmt(row.createTime) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="230" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="view(row)">查看</el-button>
          <template v-if="row.status === 'pending'">
            <el-button link type="success" @click="audit(row, true)">通过</el-button>
            <el-button link type="danger" @click="audit(row, false)">驳回</el-button>
          </template>
          <el-button v-if="row.status !== 'pending'" link type="danger" @click="del(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="!loading && !list.length" description="暂无公告" />

    <el-dialog v-model="dialog" :title="current.title" width="620px">
      <div class="meta">{{ current.publisherName }} · {{ fmt(current.createTime) }}</div>
      <div class="content">{{ current.content }}</div>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import api from '../api'

const list = ref([])
const loading = ref(false)
const tab = ref('pending')
const dialog = ref(false)
const current = reactive({ title: '', content: '', publisherName: '', createTime: '' })

const catName = c => ({ notice: '通知', teaching: '教务', life: '生活', activity: '活动' }[c] || c)
const stName = s => ({ pending: '待审核', published: '已发布', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'warning', published: 'success', rejected: 'danger' }[s] || 'info')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try { list.value = await api.get('/announcements/manage', { params: tab.value ? { status: tab.value } : {} }) }
  finally { loading.value = false }
}
async function audit(row, approve) {
  await api.post(`/announcements/${row.id}/audit`, { approve })
  ElMessage.success(approve ? '已通过并发布' : '已驳回')
  load()
}
async function del(row) {
  await ElMessageBox.confirm(`确定删除公告「${row.title}」？`, '提示', { type: 'warning' })
  await api.delete(`/announcements/${row.id}`)
  ElMessage.success('已删除')
  load()
}
function view(row) { Object.assign(current, row); dialog.value = true }
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.meta { color: #909399; font-size: 13px; margin-bottom: 12px; }
.content { white-space: pre-wrap; line-height: 1.9; }
</style>
