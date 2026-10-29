<template>
  <el-card shadow="never">
    <template #header>
      <div class="hd">
        <span>校园公告</span>
        <div>
          <el-select v-model="category" placeholder="全部分类" clearable style="width:130px;margin-right:10px" @change="reload">
            <el-option label="通知" value="notice" />
            <el-option label="教务" value="teaching" />
            <el-option label="生活" value="life" />
            <el-option label="活动" value="activity" />
          </el-select>
          <el-input v-model="keyword" placeholder="搜索标题/内容" style="width:200px;margin-right:10px" clearable @keyup.enter="reload" />
          <el-button type="primary" :icon="Search" @click="reload">搜索</el-button>
        </div>
      </div>
    </template>

    <div v-loading="loading">
      <div v-for="a in list" :key="a.id" class="item" @click="open(a)">
        <div class="row1">
          <el-tag v-if="a.isTop" type="danger" size="small">置顶</el-tag>
          <span class="title">{{ a.title }}</span>
          <el-tag size="small" type="info" effect="plain">{{ catName(a.category) }}</el-tag>
        </div>
        <div class="row2">
          <span>{{ a.publisherName }}</span>
          <span>{{ fmt(a.createTime) }}</span>
          <span>浏览 {{ a.viewCount }}</span>
        </div>
      </div>
      <el-empty v-if="!loading && !list.length" description="暂无公告" />
    </div>

    <el-pagination
      style="margin-top:16px;justify-content:flex-end"
      layout="total, prev, pager, next"
      :total="total" :page-size="pageSize" :current-page="pageIndex"
      @current-change="onPage" />

    <el-dialog v-model="dialog" :title="current.title" width="620px">
      <div class="meta">{{ current.publisherName }} · {{ fmt(current.createTime) }}</div>
      <div class="content">{{ current.content }}</div>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { Search } from '@element-plus/icons-vue'
import api from '../api'

const list = ref([])
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)
const category = ref('')
const keyword = ref('')
const loading = ref(false)
const dialog = ref(false)
const current = reactive({ title: '', content: '', publisherName: '', createTime: '' })

const catName = c => ({ notice: '通知', teaching: '教务', life: '生活', activity: '活动' }[c] || c)
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() {
  loading.value = true
  try {
    const params = { pageIndex: pageIndex.value, pageSize: pageSize.value }
    if (category.value) params.category = category.value
    if (keyword.value) params.keyword = keyword.value
    const d = await api.get('/announcements', { params })
    list.value = d.list
    total.value = d.total
  } finally { loading.value = false }
}
function reload() { pageIndex.value = 1; load() }
function onPage(p) { pageIndex.value = p; load() }

async function open(a) {
  const d = await api.get(`/announcements/${a.id}`)
  Object.assign(current, d)
  dialog.value = true
}

onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
.item { padding: 14px 6px; border-bottom: 1px solid #f0f2f5; cursor: pointer; }
.item:hover { background: #fafcff; }
.row1 { display: flex; align-items: center; gap: 8px; }
.title { font-size: 15px; font-weight: 500; color: #303133; }
.row2 { display: flex; gap: 18px; color: #909399; font-size: 12px; margin-top: 6px; }
.meta { color: #909399; font-size: 13px; margin-bottom: 12px; }
.content { white-space: pre-wrap; line-height: 1.9; color: #303133; }
</style>
