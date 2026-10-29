<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>消息中心 <el-badge :value="unread" :hidden="unread === 0" /></span>
        <div>
          <el-radio-group v-model="type" size="small" @change="load" style="margin-right:10px">
            <el-radio-button label="">全部</el-radio-button>
            <el-radio-button label="repair">报修</el-radio-button>
            <el-radio-button label="announce">公告</el-radio-button>
            <el-radio-button label="ai">AI提醒</el-radio-button>
            <el-radio-button label="system">系统</el-radio-button>
          </el-radio-group>
          <el-button size="small" @click="readAll">全部已读</el-button>
        </div>
      </div>
    </template>

    <div v-loading="loading">
      <div v-for="n in list" :key="n.id" :class="['msg', { unread: !n.isRead }]" @click="read(n)">
        <div class="dot" :class="n.type" />
        <div class="body">
          <div class="title">{{ n.title }}</div>
          <div class="content">{{ n.content }}</div>
          <div class="time">{{ fmt(n.createTime) }}</div>
        </div>
        <el-tag v-if="!n.isRead" size="small" type="danger">未读</el-tag>
      </div>
      <el-empty v-if="!loading && !list.length" description="暂无消息" />
    </div>
  </el-card>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import api from '../api'

const list = ref([])
const type = ref('')
const loading = ref(false)
const unread = ref(0)
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try {
    list.value = await api.get('/notifications', { params: type.value ? { type: type.value } : {} })
    const d = await api.get('/notifications/unread-count')
    unread.value = d.count
  } finally { loading.value = false }
}
async function read(n) {
  if (n.isRead) return
  await api.post(`/notifications/${n.id}/read`)
  n.isRead = true
  const d = await api.get('/notifications/unread-count')
  unread.value = d.count
}
async function readAll() {
  await api.post('/notifications/read-all')
  load()
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.msg { display: flex; gap: 12px; padding: 14px 8px; border-bottom: 1px solid #f0f2f5; cursor: pointer; align-items: flex-start; }
.msg:hover { background: #fafcff; }
.msg.unread { background: #f5faff; }
.dot { width: 10px; height: 10px; border-radius: 50%; margin-top: 6px; flex-shrink: 0; background: #909399; }
.dot.repair { background: #e6a23c; }
.dot.announce { background: #409eff; }
.dot.ai { background: #a06be0; }
.dot.system { background: #67c23a; }
.body { flex: 1; }
.title { font-weight: 500; color: #303133; }
.content { color: #606266; font-size: 13px; margin-top: 4px; }
.time { color: #c0c4cc; font-size: 12px; margin-top: 4px; }
</style>
