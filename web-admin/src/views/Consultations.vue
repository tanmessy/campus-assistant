<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>学生咨询（来自 AI 助手的问题提交）</span>
        <el-select v-model="status" placeholder="全部状态" clearable style="width:130px" @change="load">
          <el-option label="待回复" value="pending" />
          <el-option label="已回复" value="replied" />
        </el-select>
      </div>
    </template>

    <el-table :data="list" stripe v-loading="loading">
      <el-table-column prop="studentName" label="学生" width="100" />
      <el-table-column prop="studentNo" label="学号" width="130" />
      <el-table-column prop="category" label="分类" width="80" />
      <el-table-column prop="question" label="问题内容" min-width="240" show-overflow-tooltip />
      <el-table-column label="状态" width="90">
        <template #default="{ row }">
          <el-tag :type="row.status === 'replied' ? 'success' : 'warning'" size="small">
            {{ row.status === 'replied' ? '已回复' : '待回复' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="提交时间" width="160">
        <template #default="{ row }">{{ fmt(row.createTime) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="110">
        <template #default="{ row }">
          <el-button link type="primary" @click="openReply(row)">{{ row.status === 'replied' ? '查看/追评' : '回复' }}</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="!loading && !list.length" description="暂无咨询" />

    <el-dialog v-model="dialog" title="回复学生咨询" width="560px">
      <div class="q"><b>学生：</b>{{ current.studentName }}（{{ current.studentNo }}）</div>
      <div class="q"><b>问题：</b>{{ current.question }}</div>
      <div v-if="current.reply" class="prev"><b>已有回复：</b>{{ current.reply }}</div>
      <el-input v-model="reply" type="textarea" :rows="4" placeholder="输入回复内容" style="margin-top:12px" />
      <template #footer>
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="submit">提交回复</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const list = ref([])
const status = ref('')
const loading = ref(false)
const dialog = ref(false)
const saving = ref(false)
const reply = ref('')
const current = reactive({ id: null, studentName: '', studentNo: '', question: '', reply: '' })
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try { list.value = await api.get('/consultations', { params: status.value ? { status: status.value } : {} }) }
  finally { loading.value = false }
}
function openReply(row) {
  Object.assign(current, row); reply.value = row.reply || ''
  dialog.value = true
}
async function submit() {
  if (!reply.value.trim()) return ElMessage.warning('请输入回复内容')
  saving.value = true
  try {
    await api.post(`/consultations/${current.id}/reply`, { reply: reply.value })
    ElMessage.success('回复成功')
    dialog.value = false
    load()
  } finally { saving.value = false }
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.q { margin-bottom: 8px; line-height: 1.7; }
.prev { background: #f0f9eb; padding: 10px; border-radius: 6px; margin-top: 8px; color: #529b2e; }
</style>
