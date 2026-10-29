<template>
  <div>
    <van-nav-bar title="我的公告" left-arrow @click-left="$router.back()" />
    <van-notice-bar wrapable :scrollable="false" text="移动端仅支持查看公告与留言，发布/编辑请在 PC 端操作。" />
    <van-cell-group inset style="margin-top:10px">
      <van-cell v-for="a in list" :key="a.id" :title="a.title"
        :label="`${stName(a.status)} · ${fmt(a.createTime)} · 浏览${a.viewCount}`" is-link @click="open(a)">
        <template #value>
          <van-tag :type="stType(a.status)">{{ stName(a.status) }}</van-tag>
        </template>
      </van-cell>
    </van-cell-group>
    <van-empty v-if="!list.length" description="暂无公告" />

    <van-popup v-model:show="show" position="bottom" round :style="{ height: '70%', padding: '16px', overflowY: 'auto' }">
      <h3>{{ cur.title }}</h3>
      <p class="meta">{{ stName(cur.status) }} · {{ fmt(cur.createTime) }}</p>
      <p class="content">{{ cur.content }}</p>
    </van-popup>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import api from '../../api'

const list = ref([])
const show = ref(false)
const cur = reactive({})
const stName = s => ({ pending: '待审核', published: '已发布', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'warning', published: 'success', rejected: 'danger' }[s] || 'default')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function load() { list.value = await api.get('/announcements/manage') }
function open(a) { Object.assign(cur, a); show.value = true }
onMounted(load)
</script>

<style scoped>
.meta { color: #969799; font-size: 13px; }
.content { white-space: pre-wrap; line-height: 1.9; }
</style>
