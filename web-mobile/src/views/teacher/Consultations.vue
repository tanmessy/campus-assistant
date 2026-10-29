<template>
  <div>
    <van-nav-bar title="学生咨询" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="status" @change="load">
      <van-tab title="待回复" name="pending" />
      <van-tab title="已回复" name="replied" />
      <van-tab title="全部" name="" />
    </van-tabs>

    <van-cell-group inset style="margin-top:10px">
      <van-cell v-for="c in list" :key="c.id" :title="`${c.studentName}：${c.question}`"
        :label="c.reply ? `回复：${c.reply}` : '待回复'" is-link @click="open(c)">
        <template #value>
          <van-tag :type="c.status === 'replied' ? 'success' : 'warning'">
            {{ c.status === 'replied' ? '已回复' : '待回复' }}
          </van-tag>
        </template>
      </van-cell>
    </van-cell-group>
    <van-empty v-if="!list.length" description="暂无咨询" />

    <van-popup v-model:show="show" position="bottom" round :style="{ padding: '16px' }">
      <div class="q"><b>学生：</b>{{ cur.studentName }}（{{ cur.studentNo }}）</div>
      <div class="q"><b>问题：</b>{{ cur.question }}</div>
      <div v-if="cur.reply" class="prev"><b>已有回复：</b>{{ cur.reply }}</div>
      <van-field v-model="reply" type="textarea" rows="3" placeholder="输入回复内容" style="margin-top:12px" />
      <van-button type="primary" block round style="margin-top:12px" :loading="saving" @click="submit">提交回复</van-button>
    </van-popup>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { showToast } from 'vant'
import api from '../../api'

const list = ref([])
const status = ref('pending')
const show = ref(false)
const reply = ref('')
const saving = ref(false)
const cur = reactive({})

async function load() { list.value = await api.get('/consultations', { params: status.value ? { status: status.value } : {} }) }
function open(c) { Object.assign(cur, c); reply.value = c.reply || ''; show.value = true }
async function submit() {
  if (!reply.value.trim()) return showToast('请输入回复内容')
  saving.value = true
  try {
    await api.post(`/consultations/${cur.id}/reply`, { reply: reply.value })
    showToast('回复成功'); show.value = false; load()
  } finally { saving.value = false }
}
onMounted(load)
</script>

<style scoped>
.q { margin-bottom: 8px; line-height: 1.7; }
.prev { background: #f0f9eb; padding: 10px; border-radius: 6px; margin-top: 8px; color: #529b2e; }
</style>
