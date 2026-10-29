<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>我的报修记录</span>
        <div>
          <el-select v-model="status" placeholder="全部状态" clearable style="width:130px;margin-right:10px" @change="load">
            <el-option label="待处理" value="pending" />
            <el-option label="处理中" value="processing" />
            <el-option label="已完工" value="done" />
            <el-option label="已驳回" value="rejected" />
          </el-select>
          <el-button type="primary" :icon="Refresh" @click="load">刷新</el-button>
        </div>
      </div>
    </template>

    <el-table :data="list" stripe v-loading="loading">
      <el-table-column prop="id" label="工单号" width="80" />
      <el-table-column prop="category" label="类型" width="90" />
      <el-table-column prop="dormitory" label="报修地点" width="140" />
      <el-table-column prop="description" label="故障描述" min-width="200" show-overflow-tooltip />
      <el-table-column label="紧急度" width="90">
        <template #default="{ row }">
          <el-tag :type="priType(row.priority)" size="small">{{ row.priority }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="stType(row.status)" size="small">{{ stName(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="提交时间" width="160">
        <template #default="{ row }">{{ fmt(row.createTime) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="180" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="showProgress(row)">进度</el-button>
          <el-button v-if="row.status === 'done' && !row.rating" link type="success" @click="openReview(row)">评价</el-button>
          <el-tag v-if="row.rating" size="small" type="warning">已评{{ row.rating }}星</el-tag>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="!loading && !list.length" description="暂无报修记录" />

    <el-drawer v-model="drawer" title="工单进度" size="440px">
      <el-timeline>
        <el-timeline-item
          v-for="p in progress" :key="p.id"
          :timestamp="fmt(p.createTime)"
          :type="stType(p.status)"
          placement="top">
          <div class="p-title">{{ stName(p.status) }} · {{ p.operatorName }}</div>
          <div v-if="p.remark" class="p-remark">{{ p.remark }}</div>
        </el-timeline-item>
      </el-timeline>
      <el-empty v-if="!progress.length" description="暂无进度" />
    </el-drawer>

    <el-dialog v-model="reviewDialog" title="服务评价" width="420px">
      <div style="margin-bottom:12px">请为本次维修服务打分：</div>
      <el-rate v-model="reviewForm.rating" />
      <el-input v-model="reviewForm.comment" type="textarea" :rows="3" placeholder="补充评价（选填）" style="margin-top:14px" />
      <template #footer>
        <el-button @click="reviewDialog = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitReview">提交评价</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { Refresh } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const list = ref([])
const status = ref('')
const loading = ref(false)
const drawer = ref(false)
const progress = ref([])
const reviewDialog = ref(false)
const submitting = ref(false)
const reviewForm = reactive({ rating: 5, comment: '', id: null })

const stName = s => ({ pending: '待处理', processing: '处理中', done: '已完工', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'warning', processing: 'primary', done: 'success', rejected: 'danger' }[s] || 'info')
const priType = p => ({ 高: 'danger', 中: 'warning', 低: 'info' }[p] || 'info')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try { list.value = await api.get('/repairs', { params: status.value ? { status: status.value } : {} }) }
  finally { loading.value = false }
}
async function showProgress(row) {
  progress.value = await api.get(`/repairs/${row.id}/progress`)
  drawer.value = true
}
function openReview(row) {
  reviewForm.id = row.id; reviewForm.rating = 5; reviewForm.comment = ''
  reviewDialog.value = true
}
async function submitReview() {
  submitting.value = true
  try {
    await api.post(`/repairs/${reviewForm.id}/review`, { rating: reviewForm.rating, comment: reviewForm.comment })
    ElMessage.success('评价成功，感谢反馈')
    reviewDialog.value = false
    load()
  } finally { submitting.value = false }
}

onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.p-title { font-weight: 500; }
.p-remark { color: #606266; font-size: 13px; margin-top: 4px; }
</style>
