<template>
  <el-row :gutter="16">
    <el-col :span="9">
      <el-card>
        <template #header>课程列表</template>
        <el-table :data="courses" highlight-current-row @current-change="select" size="small">
          <el-table-column prop="name" label="课程名" />
          <el-table-column prop="teacherName" label="教师" width="80" />
          <el-table-column prop="schedule" label="时间" width="110" />
        </el-table>
      </el-card>
    </el-col>
    <el-col :span="15">
      <el-card>
        <template #header>
          <span>{{ current ? current.name + ' · 选课学生' : '请选择课程' }}</span>
        </template>
        <el-table :data="students" v-if="current" size="small">
          <el-table-column prop="name" label="姓名" width="100" />
          <el-table-column prop="studentNo" label="学号" width="130" />
          <el-table-column label="成绩" width="100">
            <template #default="{ row }">
              <span :style="{ color: row.score != null && row.score < 60 ? '#E24B4A' : 'inherit' }">
                {{ row.score ?? '未录入' }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="预警" width="80">
            <template #default="{ row }">
              <el-tag v-if="row.hasAlert" type="danger" size="small">有预警</el-tag>
              <span v-else style="color:#c0c4cc">—</span>
            </template>
          </el-table-column>
          <el-table-column label="录入成绩">
            <template #default="{ row }">
              <el-input-number v-model="row._score" :min="0" :max="100" size="small" style="width:110px" />
              <el-button size="small" type="primary" style="margin-left:8px" @click="saveScore(row)">保存</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-else description="点击左侧课程查看选课学生" />
      </el-card>
      <el-card style="margin-top:16px">
        <template #header>课程预警学生名单</template>
        <el-table :data="alerts" size="small">
          <el-table-column prop="name" label="姓名" width="100" />
          <el-table-column prop="studentNo" label="学号" width="130" />
          <el-table-column prop="courseName" label="课程" />
          <el-table-column prop="scoreValue" label="成绩" width="80" />
          <el-table-column prop="reason" label="预警原因" />
        </el-table>
      </el-card>
    </el-col>
  </el-row>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const courses = ref([])
const students = ref([])
const alerts = ref([])
const current = ref(null)

async function loadCourses() { courses.value = await api.get('/courses') }
async function loadAlerts() { alerts.value = await api.get('/alerts') }

async function select(row) {
  current.value = row
  if (!row) return
  students.value = (await api.get(`/courses/${row.id}/students`)).map(s => ({ ...s, _score: s.score }))
}

async function saveScore(row) {
  if (row._score == null) return ElMessage.warning('请输入成绩')
  const res = await api.post('/scores', { studentId: row.id, courseId: current.value.id, value: row._score, semester: current.value.semester })
  ElMessage.success(res.alertGenerated ? '已保存，并自动生成学业预警' : '成绩已保存')
  select(current.value)
  loadAlerts()
}

onMounted(() => { loadCourses(); loadAlerts() })
</script>
