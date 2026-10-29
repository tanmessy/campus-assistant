<template>
  <div>
    <van-nav-bar title="食堂菜单" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="mealType" @change="load">
      <van-tab title="早餐" name="早餐" />
      <van-tab title="午餐" name="午餐" />
      <van-tab title="晚餐" name="晚餐" />
    </van-tabs>

    <van-cell-group v-for="(group, canteen) in grouped" :key="canteen" :title="canteen" inset style="margin-top:12px">
      <van-cell v-for="m in group" :key="m.id" :title="m.dishName" :label="`${m.window} · ${m.description || ''}`">
        <template #value>
          <span class="price">¥{{ m.price }}</span>
        </template>
      </van-cell>
    </van-cell-group>

    <van-empty v-if="!Object.keys(grouped).length" description="该餐次暂无菜单" />
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import api from '../api'

const mealType = ref('午餐')
const list = ref([])

const grouped = computed(() => {
  const g = {}
  list.value.forEach(m => { (g[m.canteen] = g[m.canteen] || []).push(m) })
  return g
})

async function load() {
  list.value = await api.get('/cafeteria', { params: { mealType: mealType.value } })
}
onMounted(load)
</script>

<style scoped>
.price { color: #ee0a24; font-weight: 600; }
</style>
