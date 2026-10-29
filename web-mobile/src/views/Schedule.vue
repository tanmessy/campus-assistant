<template>
  <div>
    <van-nav-bar title="课表与成绩" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="tab">
      <van-tab title="我的课表">
        <van-cell-group inset style="margin-top:12px">
          <van-cell v-for="c in courses" :key="c.id" :title="c.name"
            :label="`${c.schedule || ''} · ${c.classroom || ''} · ${c.teacherName}`"
            :value="`${c.credit} 学分`" />
          <van-empty v-if="!courses.length" description="暂无课程" />
        </van-cell-group>
      </van-tab>
      <van-tab title="我的成绩">
        <van-cell-group inset style="margin-top:12px">
          <van-cell v-for="(s, i) in scores" :key="i" :title="s.name" :label="s.semester">
            <template #value>
              <span :style="{ color: s.value < 60 ? '#ee0a24' : '#07c160', fontWeight: 500 }">{{ s.value }} 分</span>
            </template>
          </van-cell>
          <van-empty v-if="!scores.length" description="暂无成绩" />
        </van-cell-group>
      </van-tab>
    </van-tabs>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../api'

const tab = ref(0)
const courses = ref([])
const scores = ref([])

onMounted(async () => {
  courses.value = await api.get('/courses')
  scores.value = await api.get('/scores/my')
})
</script>
