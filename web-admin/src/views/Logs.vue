<template>
  <el-tabs v-model="tab" @tab-change="load">
    <el-tab-pane label="登录日志" name="login">
      <el-card shadow="never">
        <template #header>
          <div class="hd">
            <span>登录日志（记录每次登录与失败，可追溯越权尝试）</span>
            <el-select v-model="loginResult" placeholder="全部结果" clearable style="width:130px" @change="loadLogin">
              <el-option label="成功" value="success" />
              <el-option label="失败" value="fail" />
            </el-select>
          </div>
        </template>
        <el-table :data="loginLogs" stripe v-loading="loading">
          <el-table-column prop="username" label="账号" width="140" />
          <el-table-column prop="ip" label="IP 地址" width="150" />
          <el-table-column label="结果" width="100">
            <template #default="{ row }">
              <el-tag :type="row.result === 'success' ? 'success' : 'danger'" size="small">
                {{ row.result === 'success' ? '成功' : '失败' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="message" label="说明" min-width="160" />
          <el-table-column label="时间" width="170">
            <template #default="{ row }">{{ fmt(row.createTime) }}</template>
          </el-table-column>
        </el-table>
        <el-pagination style="margin-top:14px;justify-content:flex-end" layout="total, prev, pager, next"
          :total="loginTotal" :page-size="15" :current-page="loginPage" @current-change="p => { loginPage = p; loadLogin() }" />
      </el-card>
    </el-tab-pane>

    <el-tab-pane label="操作日志" name="operation">
      <el-card shadow="never">
        <template #header>
          <div class="hd">
            <span>操作日志（记录关键管理操作）</span>
            <el-input v-model="opKeyword" placeholder="搜索模块/操作" style="width:200px" clearable @keyup.enter="loadOp" />
          </div>
        </template>
        <el-table :data="opLogs" stripe v-loading="loading">
          <el-table-column prop="username" label="操作人" width="120" />
          <el-table-column prop="module" label="模块" width="120" />
          <el-table-column prop="action" label="操作" width="120" />
          <el-table-column prop="detail" label="详情" min-width="220" show-overflow-tooltip />
          <el-table-column prop="ip" label="IP" width="140" />
          <el-table-column label="时间" width="170">
            <template #default="{ row }">{{ fmt(row.createTime) }}</template>
          </el-table-column>
        </el-table>
        <el-pagination style="margin-top:14px;justify-content:flex-end" layout="total, prev, pager, next"
          :total="opTotal" :page-size="15" :current-page="opPage" @current-change="p => { opPage = p; loadOp() }" />
      </el-card>
    </el-tab-pane>
  </el-tabs>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import api from '../api'

const tab = ref('login')
const loading = ref(false)
const loginLogs = ref([])
const opLogs = ref([])
const loginResult = ref('')
const opKeyword = ref('')
const loginPage = ref(1); const loginTotal = ref(0)
const opPage = ref(1); const opTotal = ref(0)
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function loadLogin() {
  loading.value = true
  try {
    const d = await api.get('/logs/login', { params: { pageIndex: loginPage.value, pageSize: 15, result: loginResult.value || undefined } })
    loginLogs.value = d.list; loginTotal.value = d.total
  } finally { loading.value = false }
}
async function loadOp() {
  loading.value = true
  try {
    const d = await api.get('/logs/operation', { params: { pageIndex: opPage.value, pageSize: 15, keyword: opKeyword.value || undefined } })
    opLogs.value = d.list; opTotal.value = d.total
  } finally { loading.value = false }
}
function load() { tab.value === 'login' ? loadLogin() : loadOp() }
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
</style>
