<template>
  <div>
    <el-row :gutter="16">
      <el-col :span="6" v-for="c in cards" :key="c.label">
        <el-card shadow="hover" class="stat-card">
          <div class="num" :style="{ color: c.color }">{{ c.value }}</div>
          <div class="label">{{ c.label }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top:16px">
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>
            <div class="hd">
              <span>🤖 AI 数据摘要</span>
              <el-button size="small" type="primary" :loading="aiLoading" @click="loadSummary">重新生成</el-button>
            </div>
          </template>
          <pre class="summary">{{ summary || '正在生成…' }}</pre>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>待办提醒</template>
          <el-alert v-if="stats.pendingAnnouncements > 0" type="warning" :closable="false" style="margin-bottom:8px"
            :title="`有 ${stats.pendingAnnouncements} 条公告待审核`" />
          <el-alert v-if="stats.pendingRepairs > 0" type="warning" :closable="false" style="margin-bottom:8px"
            :title="`有 ${stats.pendingRepairs} 条报修工单待处理`" />
          <el-alert v-if="stats.pendingBookings > 0" type="info" :closable="false" style="margin-bottom:8px"
            :title="`有 ${stats.pendingBookings} 条场地预约待审核`" />
          <el-alert v-if="stats.alertCount > 0" type="error" :closable="false"
            :title="`共有 ${stats.alertCount} 条学业预警记录`" />
          <el-empty v-if="!stats.pendingAnnouncements && !stats.pendingRepairs && !stats.pendingBookings && !stats.alertCount" description="暂无待办事项" :image-size="70" />
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top:16px">
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>报修类型分布</template>
          <div v-for="c in report.byCategory" :key="c.name" class="bar-row">
            <span class="bar-label">{{ c.name }}</span>
            <el-progress :percentage="pct(c.count)" :stroke-width="16" :show-text="false" style="flex:1" />
            <span class="bar-val">{{ c.count }}</span>
          </div>
          <el-empty v-if="!report.byCategory?.length" description="暂无数据" :image-size="60" />
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>近 7 天报修趋势</template>
          <div class="trend">
            <div v-for="t in report.trend" :key="t.date" class="tcol">
              <div class="tbar" :style="{ height: trendH(t.count) }">{{ t.count }}</div>
              <div class="tlabel">{{ t.date }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import api from '../api'

const stats = ref({})
const report = reactive({ byCategory: [], trend: [] })
const summary = ref('')
const aiLoading = ref(false)

const cards = computed(() => [
  { label: '用户总数', value: stats.value.userCount ?? '-', color: '#185FA5' },
  { label: '学生 / 教师', value: `${stats.value.studentCount ?? '-'} / ${stats.value.teacherCount ?? '-'}`, color: '#0F6E56' },
  { label: '待处理工单', value: stats.value.pendingRepairs ?? '-', color: '#BA7517' },
  { label: '今日登录', value: stats.value.todayLogins ?? '-', color: '#534AB7' }
])

const pct = n => Math.round(n * 100 / Math.max(...report.byCategory.map(x => x.count), 1))
const trendH = n => Math.max(8, n * 26) + 'px'

async function loadSummary() {
  aiLoading.value = true
  try { const d = await api.get('/ai/dashboard-summary'); summary.value = d.summary } finally { aiLoading.value = false }
}

onMounted(async () => {
  stats.value = await api.get('/stats/dashboard')
  Object.assign(report, await api.get('/stats/repair-report'))
  loadSummary()
})
</script>

<style scoped>
.stat-card { text-align: center; }
.num { font-size: 30px; font-weight: 600; }
.label { color: #909399; margin-top: 6px; }
.hd { display: flex; justify-content: space-between; align-items: center; }
.summary { margin: 0; white-space: pre-wrap; font-family: inherit; line-height: 1.9; color: #303133; }
.bar-row { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; }
.bar-label { width: 60px; color: #606266; }
.bar-val { width: 40px; text-align: right; color: #909399; }
.trend { display: flex; align-items: flex-end; gap: 14px; height: 160px; padding-top: 10px; }
.tcol { flex: 1; display: flex; flex-direction: column; align-items: center; justify-content: flex-end; }
.tbar { width: 100%; background: #409eff; border-radius: 5px 5px 0 0; color: #fff; font-size: 12px; text-align: center; min-height: 8px; }
.tlabel { margin-top: 6px; font-size: 12px; color: #909399; }
</style>
