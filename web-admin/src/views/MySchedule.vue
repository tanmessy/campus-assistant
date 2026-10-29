<template>
  <div>
    <el-card shadow="never">
      <template #header>
        <div class="hd">
          <span>我的课表</span>
          <div>
            <el-select v-model="semester" placeholder="学期" style="width:150px;margin-right:10px" @change="load">
              <el-option v-for="s in semesters" :key="s" :label="s" :value="s" />
            </el-select>
            <el-button type="primary" :icon="Download" @click="exportExcel">导出课表</el-button>
          </div>
        </div>
      </template>

      <el-table :data="filtered" stripe v-loading="loading">
        <el-table-column prop="name" label="课程名称" min-width="160" />
        <el-table-column prop="teacherName" label="任课教师" width="110" />
        <el-table-column prop="classroom" label="上课地点" width="130" />
        <el-table-column prop="schedule" label="上课时间" width="130" />
        <el-table-column prop="semester" label="学期" width="140" />
        <el-table-column prop="credit" label="学分" width="80" />
      </el-table>

      <el-empty v-if="!loading && !filtered.length" description="暂无课表数据" />
    </el-card>

    <el-card shadow="never" style="margin-top:16px">
      <template #header><span>我的成绩与学业预警</span></template>
      <el-table :data="scores" stripe v-loading="loading">
        <el-table-column prop="name" label="课程" min-width="150" />
        <el-table-column prop="semester" label="学期" width="140" />
        <el-table-column label="成绩" width="100">
          <template #default="{ row }">
            <el-tag :type="row.value >= 60 ? 'success' : 'danger'">{{ row.value }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="credit" label="学分" width="80" />
      </el-table>

      <el-alert v-if="failedCount > 0" type="warning" :closable="false" style="margin-top:14px">
        <div style="display:flex;justify-content:space-between;align-items:center">
          <span>您有 {{ failedCount }} 门课程不及格，建议查看 AI 学习建议</span>
          <el-button size="small" type="primary" :loading="adviceLoading" @click="getAdvice">AI 学习建议</el-button>
        </div>
      </el-alert>
      <div v-if="advice" class="advice">
        <div class="advice-title">🤖 AI 学习建议</div>
        <pre>{{ advice }}</pre>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { Download } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const courses = ref([])
const scores = ref([])
const semester = ref('')
const loading = ref(false)
const advice = ref('')
const adviceLoading = ref(false)

const semesters = computed(() => [...new Set(courses.value.map(c => c.semester).filter(Boolean))])
const filtered = computed(() => semester.value ? courses.value.filter(c => c.semester === semester.value) : courses.value)
const failedCount = computed(() => scores.value.filter(s => s.value < 60).length)

async function load() {
  loading.value = true
  try {
    courses.value = await api.get('/courses')
    scores.value = await api.get('/scores/my')
  } finally { loading.value = false }
}

async function getAdvice() {
  adviceLoading.value = true
  try { const d = await api.get('/ai/study-advice'); advice.value = d.advice } finally { adviceLoading.value = false }
}

// 课表导出为 Excel（CSV，带 BOM，Excel 可直接打开）
function exportExcel() {
  const rows = [['课程名称', '任课教师', '上课地点', '上课时间', '学期', '学分']]
  filtered.value.forEach(c => rows.push([c.name, c.teacherName, c.classroom, c.schedule, c.semester, c.credit]))
  const csv = rows.map(r => r.map(v => `"${v ?? ''}"`).join(',')).join('\n')
  const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8' })
  const a = document.createElement('a')
  a.href = URL.createObjectURL(blob)
  a.download = `我的课表_${new Date().toLocaleDateString()}.csv`
  a.click()
  ElMessage.success('课表已导出')
}

onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.advice { margin-top: 14px; background: #f0f7ff; border-radius: 8px; padding: 14px 16px; }
.advice-title { font-weight: 600; margin-bottom: 6px; color: #409eff; }
.advice pre { margin: 0; white-space: pre-wrap; font-family: inherit; line-height: 1.8; color: #303133; }
</style>
