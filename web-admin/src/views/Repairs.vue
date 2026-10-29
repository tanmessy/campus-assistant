<template>
  <el-card shadow="never" v-loading="loading">
    <template #header>
      <div class="hd">
        <span>报修工单管理</span>
        <div>
          <el-input v-model="keyword" placeholder="搜索宿舍/描述" style="width:190px;margin-right:10px" clearable @keyup.enter="reload" />
          <el-radio-group v-model="status" @change="reload">
            <el-radio-button label="">全部</el-radio-button>
            <el-radio-button label="pending">待接单</el-radio-button>
            <el-radio-button label="processing">处理中</el-radio-button>
            <el-radio-button label="done">已完工</el-radio-button>
          </el-radio-group>
        </div>
      </div>
    </template>

    <el-table :data="list" stripe>
      <el-table-column prop="id" label="单号" width="70" />
      <el-table-column prop="studentName" label="报修人" width="90" />
      <el-table-column prop="dormitory" label="报修地点" width="130" />
      <el-table-column prop="description" label="故障描述" min-width="170" show-overflow-tooltip />
      <el-table-column label="AI分类" width="85">
        <template #default="{ row }"><el-tag effect="plain">{{ row.category }}</el-tag></template>
      </el-table-column>
      <el-table-column label="紧急度" width="85">
        <template #default="{ row }">
          <el-tag :type="priType(row.priority)">{{ row.priority }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="aiAdvice" label="AI 维修建议" min-width="150" show-overflow-tooltip />
      <el-table-column label="状态" width="90">
        <template #default="{ row }"><el-tag :type="stType(row.status)">{{ stName(row.status) }}</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="230" fixed="right">
        <template #default="{ row }">
          <el-button size="small" link type="primary" @click="detail(row)">详情</el-button>
          <el-button v-if="row.status === 'pending'" size="small" type="primary" @click="handle(row, 'accept')">接单</el-button>
          <el-button v-if="row.status === 'pending'" size="small" @click="openAssign(row)">分配</el-button>
          <el-button v-if="row.status === 'processing'" size="small" type="success" @click="openProgress(row)">更新进度</el-button>
          <el-button v-if="row.status === 'processing'" size="small" type="success" plain @click="handle(row, 'done')">完工</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination style="margin-top:16px;justify-content:flex-end" layout="total, prev, pager, next"
      :total="total" :page-size="pageSize" :current-page="pageIndex" @current-change="onPage" />

    <!-- 详情抽屉 -->
    <el-drawer v-model="drawer" title="工单详情" size="480px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="单号">{{ cur.id }}</el-descriptions-item>
        <el-descriptions-item label="报修人">{{ cur.studentName }}</el-descriptions-item>
        <el-descriptions-item label="报修地点">{{ cur.dormitory }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ cur.contact || '未填写' }}</el-descriptions-item>
        <el-descriptions-item label="故障描述">{{ cur.description }}</el-descriptions-item>
        <el-descriptions-item label="AI 分类/紧急度">{{ cur.category }} / {{ cur.priority }}</el-descriptions-item>
        <el-descriptions-item label="AI 维修建议">{{ cur.aiAdvice }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ stName(cur.status) }}</el-descriptions-item>
        <el-descriptions-item label="处理人">{{ cur.handlerName || '未分配' }}</el-descriptions-item>
        <el-descriptions-item label="服务评价" v-if="cur.rating">{{ cur.rating }} 星 {{ cur.comment || '' }}</el-descriptions-item>
      </el-descriptions>
      <el-divider>进度时间轴</el-divider>
      <el-timeline>
        <el-timeline-item v-for="p in cur.progress || []" :key="p.id" :timestamp="fmt(p.createTime)" :type="stType(p.status)" placement="top">
          <div class="p-t">{{ stName(p.status) }} · {{ p.operatorName }}</div>
          <div v-if="p.remark" class="p-r">{{ p.remark }}</div>
          <el-image v-if="p.images" :src="firstImg(p.images)" style="width:90px;margin-top:6px;border-radius:6px" fit="cover" />
        </el-timeline-item>
      </el-timeline>
    </el-drawer>

    <!-- 分配 -->
    <el-dialog v-model="assignDialog" title="分配工单" width="420px">
      <el-select v-model="assignForm.assignTo" placeholder="选择后勤人员" style="width:100%">
        <el-option v-for="l in logisticsList" :key="l.id" :label="`${l.name}（${l.username}）`" :value="l.id" />
      </el-select>
      <el-input v-model="assignForm.remark" type="textarea" :rows="3" placeholder="分配说明（选填）" style="margin-top:12px" />
      <template #footer>
        <el-button @click="assignDialog = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="doAssign">确定分配</el-button>
      </template>
    </el-dialog>

    <!-- 更新进度 -->
    <el-dialog v-model="progressDialog" title="更新维修进度" width="480px">
      <el-form label-width="80px">
        <el-form-item label="状态">
          <el-select v-model="progressForm.status" style="width:100%">
            <el-option label="处理中" value="processing" />
            <el-option label="已完工" value="done" />
          </el-select>
        </el-form-item>
        <el-form-item label="进度说明">
          <el-input v-model="progressForm.remark" type="textarea" :rows="3" placeholder="如：已更换配件，正在测试" />
        </el-form-item>
        <el-form-item label="现场照片">
          <el-upload :show-file-list="false" :http-request="doUpload" accept="image/*">
            <el-button :icon="Upload">上传照片</el-button>
            <span v-if="progressForm.images" style="margin-left:10px;color:#67c23a">已上传 1 张</span>
          </el-upload>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="progressDialog = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="doProgress">提交</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { Upload } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const list = ref([])
const loading = ref(false)
const status = ref('')
const keyword = ref('')
const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const drawer = ref(false)
const cur = ref({})
const assignDialog = ref(false)
const progressDialog = ref(false)
const saving = ref(false)
const logisticsList = ref([])
const assignForm = reactive({ id: null, assignTo: null, remark: '' })
const progressForm = reactive({ id: null, status: 'processing', remark: '', images: '' })

const stName = s => ({ pending: '待接单', processing: '处理中', done: '已完工', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'warning', processing: 'primary', done: 'success', rejected: 'danger' }[s] || 'info')
const priType = p => ({ 高: 'danger', 中: 'warning', 低: 'info' }[p] || 'info')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''
const firstImg = s => (s || '').split(',')[0]

async function load() {
  loading.value = true
  try {
    const d = await api.get('/repairs/page', { params: { pageIndex: pageIndex.value, pageSize: pageSize.value, status: status.value || undefined, keyword: keyword.value || undefined } })
    list.value = d.list; total.value = d.total
  } finally { loading.value = false }
}
function reload() { pageIndex.value = 1; load() }
function onPage(p) { pageIndex.value = p; load() }

async function handle(row, action) {
  await api.post(`/repairs/${row.id}/handle`, { action })
  ElMessage.success(action === 'accept' ? '已接单' : '已完工')
  load()
}
async function detail(row) {
  cur.value = await api.get(`/repairs/${row.id}`)
  drawer.value = true
}
async function openAssign(row) {
  if (!logisticsList.value.length) logisticsList.value = await api.get('/repairs/handlers')
  assignForm.id = row.id; assignForm.assignTo = null; assignForm.remark = ''
  assignDialog.value = true
}
async function doAssign() {
  if (!assignForm.assignTo) return ElMessage.warning('请选择后勤人员')
  saving.value = true
  try {
    await api.post(`/repairs/${assignForm.id}/handle`, { action: 'assign', assignTo: assignForm.assignTo, remark: assignForm.remark })
    ElMessage.success('分配成功'); assignDialog.value = false; load()
  } finally { saving.value = false }
}
function openProgress(row) {
  progressForm.id = row.id; progressForm.status = 'processing'; progressForm.remark = ''; progressForm.images = ''
  progressDialog.value = true
}
async function doUpload({ file }) {
  const fd = new FormData()
  fd.append('file', file)
  const d = await api.post('/upload', fd)
  progressForm.images = d.url
  ElMessage.success('照片已上传')
}
async function doProgress() {
  saving.value = true
  try {
    await api.post(`/repairs/${progressForm.id}/progress`, { status: progressForm.status, remark: progressForm.remark, images: progressForm.images })
    if (progressForm.status === 'done') await api.post(`/repairs/${progressForm.id}/handle`, { action: 'done', remark: progressForm.remark })
    ElMessage.success('进度已更新'); progressDialog.value = false; load()
  } finally { saving.value = false }
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.p-t { font-weight: 500; }
.p-r { color: #606266; font-size: 13px; margin-top: 4px; }
</style>
