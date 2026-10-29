<template>
  <div>
    <van-nav-bar title="校园公告" left-arrow @click-left="$router.back()" />
    <van-search v-model="keyword" placeholder="搜索公告" @search="reload" />
    <van-dropdown-menu>
      <van-dropdown-item v-model="category" :options="cats" @change="reload" />
    </van-dropdown-menu>

    <van-list v-model:loading="loading" :finished="finished" finished-text="没有更多了" @load="load">
      <van-cell v-for="a in list" :key="a.id" :title="a.title" :label="`${a.publisherName} · ${fmt(a.createTime)} · 浏览${a.viewCount}`"
        is-link @click="open(a)">
        <template #value>
          <van-tag v-if="a.isTop" type="danger">置顶</van-tag>
        </template>
      </van-cell>
    </van-list>

    <van-popup v-model:show="show" position="bottom" round :style="{ height: '70%', padding: '16px', overflowY: 'auto' }">
      <h3>{{ cur.title }}</h3>
      <p class="meta">{{ cur.publisherName }} · {{ fmt(cur.createTime) }}</p>
      <p class="content">{{ cur.content }}</p>
    </van-popup>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import api from '../api'

const list = ref([])
const loading = ref(false)
const finished = ref(false)
const pageIndex = ref(1)
const keyword = ref('')
const category = ref('')
const show = ref(false)
const cur = reactive({})
const cats = [
  { text: '全部分类', value: '' }, { text: '通知', value: 'notice' },
  { text: '教务', value: 'teaching' }, { text: '生活', value: 'life' }, { text: '活动', value: 'activity' }
]
const fmt = t => t ? new Date(t).toLocaleDateString('zh-CN') : ''

async function load() {
  const d = await api.get('/announcements', { params: { pageIndex: pageIndex.value, pageSize: 10, keyword: keyword.value || undefined, category: category.value || undefined } })
  list.value.push(...d.list)
  loading.value = false
  if (list.value.length >= d.total) finished.value = true
  else pageIndex.value++
}
function reload() { list.value = []; pageIndex.value = 1; finished.value = false; loading.value = true; load() }
async function open(a) {
  Object.assign(cur, await api.get(`/announcements/${a.id}`))
  show.value = true
}
onMounted(reload)
</script>

<style scoped>
.meta { color: #969799; font-size: 13px; }
.content { white-space: pre-wrap; line-height: 1.9; color: #323233; }
</style>
