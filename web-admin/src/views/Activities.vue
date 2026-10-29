<template>
  <div>
    <el-card>
      <template #header>
        <div style="display:flex;justify-content:space-between;align-items:center">
          <span>活动管理</span>
          <el-button type="primary" @click="dialog = true">发布活动</el-button>
        </div>
      </template>
      <el-table :data="activities" stripe size="small">
        <el-table-column prop="title" label="活动名称" min-width="160" />
        <el-table-column prop="location" label="地点" width="130" />
        <el-table-column prop="startTime" label="开始时间" width="160" />
        <el-table-column label="报名/名额" width="100">
          <template #default="{ row }">{{ row.signed }} / {{ row.quota }}</template>
        </el-table-column>
        <el-table-column label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="{ draft: 'info', published: 'success', ended: 'warning' }[row.status]">
              {{ { draft: '草稿', published: '已发布', ended: '已结束' }[row.status] }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-card style="margin-top:16px">
      <template #header>场地预约审核</template>
      <el-table :data="bookings" stripe size="small">
        <el-table-column prop="venueName" label="场地" width="140" />
        <el-table-column prop="userName" label="预约人" width="100" />
        <el-table-column label="时间段" min-width="220">
          <template #default="{ row }">{{ fmtDT(row.startTime) }} ~ {{ fmtDT(row.endTime) }}</template>
        </el-table-column>
        <el-table-column prop="reason" label="事由" min-width="140" show-overflow-tooltip />
        <el-table-column label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="{ pending: 'warning', approved: 'success', rejected: 'info' }[row.status]">
              {{ { pending: '待审核', approved: '通过', rejected: '拒绝' }[row.status] }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="140">
          <template #default="{ row }">
            <template v-if="row.status === 'pending'">
              <el-button size="small" type="success" @click="review(row, true)">通过</el-button>
              <el-button size="small" type="danger" @click="review(row, false)">拒绝</el-button>
            </template>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialog" title="发布活动" width="480px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="内容"><el-input v-model="form.content" type="textarea" :rows="3" /></el-form-item>
        <el-form-item label="地点"><el-input v-model="form.location" /></el-form-item>
        <el-form-item label="开始时间">
          <el-date-picker v-model="form.startTime" type="datetime" style="width:100%" />
        </el-form-item>
        <el-form-item label="名额"><el-input-number v-model="form.quota" :min="1" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" @click="create">发布</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const activities = ref([])
const bookings = ref([])
const dialog = ref(false)
const form = reactive({ title: '', content: '', location: '', startTime: null, quota: 50, status: 'published' })

function fmtDT(s) { return (s || '').replace('T', ' ').slice(0, 16) }

async function load() {
  activities.value = await api.get('/activities/manage')
  bookings.value = await api.get('/venues/bookings')
}
async function create() {
  await api.post('/activities', form)
  ElMessage.success('发布成功')
  dialog.value = false
  load()
}
async function review(row, approve) {
  await api.post(`/venues/bookings/${row.id}/review`, { approve })
  ElMessage.success('已处理')
  load()
}
onMounted(load)
</script>
