<template>
  <div>
    <van-nav-bar title="报修工单" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="status" @change="load">
      <van-tab title="待接单" name="pending" />
      <van-tab title="处理中" name="processing" />
      <van-tab title="已完工" name="done" />
      <van-tab title="全部" name="" />
    </van-tabs>

    <van-pull-refresh v-model="refreshing" @refresh="onRefresh">
      <van-list v-model:loading="loading" :finished="true">
        <van-cell v-for="o in list" :key="o.id" :title="`#${o.id} ${o.dormitory}`"
          :label="o.description" is-link :to="`/l-order/${o.id}`">
          <template #value>
            <van-tag :type="priType(o.priority)">{{ o.category }}·{{ o.priority }}</van-tag>
          </template>
        </van-cell>
      </van-list>
    </van-pull-refresh>
    <van-empty v-if="!loading && !list.length" description="暂无工单" />
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import api from '../../api'

const list = ref([])
const status = ref('pending')
const loading = ref(false)
const refreshing = ref(false)
const priType = p => ({ 高: 'danger', 中: 'warning', 低: 'primary' }[p] || 'primary')

async function load() {
  loading.value = true
  try { list.value = await api.get('/repairs', { params: status.value ? { status: status.value } : {} }) }
  finally { loading.value = false; refreshing.value = false }
}
function onRefresh() { refreshing.value = true; load() }
onMounted(load)
</script>
