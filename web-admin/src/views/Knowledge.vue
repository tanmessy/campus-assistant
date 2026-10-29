<template>
  <div>
    <el-card>
      <template #header>
        <div style="display:flex;justify-content:space-between;align-items:center">
          <span>AI 知识库管理</span>
          <el-button type="primary" @click="openEdit(null)">录入知识</el-button>
        </div>
      </template>
      <el-table :data="kb" stripe size="small">
        <el-table-column prop="title" label="标题" width="180" />
        <el-table-column prop="category" label="分类" width="90" />
        <el-table-column prop="content" label="内容" min-width="240" show-overflow-tooltip />
        <el-table-column label="启用" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isEnabled ? 'success' : 'info'">{{ row.isEnabled ? '启用' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="140">
          <template #default="{ row }">
            <el-button size="small" @click="openEdit(row)">编辑</el-button>
            <el-button size="small" type="danger" @click="del(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-card style="margin-top:16px">
      <template #header>AI 对话记录（最近 200 条）</template>
      <el-table :data="logs" stripe size="small">
        <el-table-column prop="userName" label="用户" width="100" />
        <el-table-column prop="question" label="问题" min-width="180" show-overflow-tooltip />
        <el-table-column prop="answer" label="AI 回答" min-width="240" show-overflow-tooltip />
        <el-table-column prop="createTime" label="时间" width="170" />
      </el-table>
    </el-card>

    <el-dialog v-model="dialog" :title="form.id ? '编辑知识' : '录入知识'" width="480px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="分类"><el-input v-model="form.category" placeholder="如：请假/报修/奖助" /></el-form-item>
        <el-form-item label="内容"><el-input v-model="form.content" type="textarea" :rows="5" /></el-form-item>
        <el-form-item label="启用"><el-switch v-model="form.isEnabled" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" @click="save">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import api from '../api'

const kb = ref([])
const logs = ref([])
const dialog = ref(false)
const form = reactive({ id: null, title: '', category: '', content: '', isEnabled: true })

async function load() {
  kb.value = await api.get('/ai/knowledge')
  logs.value = await api.get('/ai/logs')
}
function openEdit(row) {
  Object.assign(form, row ? { id: row.id, title: row.title, category: row.category, content: row.content, isEnabled: row.isEnabled }
    : { id: null, title: '', category: '', content: '', isEnabled: true })
  dialog.value = true
}
async function save() {
  if (form.id) await api.put(`/ai/knowledge/${form.id}`, form)
  else await api.post('/ai/knowledge', form)
  ElMessage.success('已保存')
  dialog.value = false
  load()
}
async function del(row) {
  await ElMessageBox.confirm(`确定删除「${row.title}」？`, '提示', { type: 'warning' })
  await api.delete(`/ai/knowledge/${row.id}`)
  load()
}
onMounted(load)
</script>
