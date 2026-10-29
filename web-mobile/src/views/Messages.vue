<template>
  <div>
    <van-nav-bar title="消息中心" left-arrow @click-left="$router.back()">
      <template #right>
        <span style="font-size:13px" @click="readAll">全部已读</span>
      </template>
    </van-nav-bar>

    <van-tabs v-model:active="type" @change="load">
      <van-tab title="全部" name="" />
      <van-tab title="报修" name="repair" />
      <van-tab title="公告" name="announce" />
      <van-tab title="AI提醒" name="ai" />
      <van-tab title="系统" name="system" />
    </van-tabs>

    <van-cell v-for="n in list" :key="n.id" :title="n.title" :label="n.content" is-link @click="read(n)">
      <template #icon>
        <van-badge :dot="!n.isRead" style="margin-right:10px">
          <van-icon :name="icon(n.type)" size="22" :color="color(n.type)" />
        </van-badge>
      </template>
      <template #value>
        <span class="time">{{ fmt(n.createTime) }}</span>
      </template>
    </van-cell>
    <van-empty v-if="!list.length" description="暂无消息" />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import api from '../api'

const list = ref([])
const type = ref('')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''
const icon = t => ({ repair: 'setting-o', announce: 'bullhorn-o', ai: 'chat-o', system: 'info-o' }[t] || 'info-o')
const color = t => ({ repair: '#ff976a', announce: '#1989fa', ai: '#7232dd', system: '#07c160' }[t] || '#969799')

async function load() {
  list.value = await api.get('/notifications', { params: type.value ? { type: type.value } : {} })
}
async function read(n) {
  if (!n.isRead) { await api.post(`/notifications/${n.id}/read`); n.isRead = true }
}
async function readAll() {
  await api.post('/notifications/read-all'); load()
}
onMounted(load)
</script>

<style scoped>
.time { font-size: 12px; color: #c8c9cc; }
</style>
