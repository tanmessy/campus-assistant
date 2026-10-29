<template>
  <div>
    <van-nav-bar title="校园活动" left-arrow @click-left="$router.back()" />
    <van-list style="padding:12px">
      <van-card v-for="a in list" :key="a.id" :title="a.title" :desc="a.content">
        <template #tags>
          <van-tag type="primary" plain>{{ a.location }}</van-tag>
          <van-tag plain style="margin-left:6px">名额 {{ a.signed }}/{{ a.quota }}</van-tag>
        </template>
        <template #footer>
          <van-button v-if="!a.mySigned" size="small" type="primary" :disabled="a.signed >= a.quota"
            @click="signup(a)">{{ a.signed >= a.quota ? '名额已满' : '报名' }}</van-button>
          <van-button v-else size="small" @click="cancel(a)">取消报名</van-button>
        </template>
      </van-card>
      <van-empty v-if="!list.length" description="暂无活动" />
    </van-list>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { showToast } from 'vant'
import api from '../api'

const list = ref([])

async function load() { list.value = await api.get('/activities') }

async function signup(a) {
  await api.post(`/activities/${a.id}/signup`)
  showToast({ type: 'success', message: '报名成功' })
  load()
}

async function cancel(a) {
  await api.post(`/activities/${a.id}/cancel`)
  showToast('已取消报名')
  load()
}

onMounted(load)
</script>
