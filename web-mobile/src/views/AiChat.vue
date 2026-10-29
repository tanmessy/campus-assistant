<template>
  <div class="chat-page">
    <van-nav-bar title="AI 智能问答" left-arrow @click-left="$router.back()" />
    <div class="messages" ref="msgBox">
      <div v-for="(m, i) in messages" :key="i" :class="['msg', m.role]">
        <div class="bubble">{{ m.text }}</div>
      </div>
      <div v-if="loading" class="msg ai"><div class="bubble">思考中…</div></div>
    </div>
    <div class="input-bar">
      <van-field v-model="input" placeholder="问一问：如何请假？如何报修？" @keyup.enter="send" />
      <van-button type="primary" :disabled="loading || !input.trim()" @click="send">发送</van-button>
    </div>
  </div>
</template>

<script setup>
import { ref, nextTick, onMounted } from 'vue'
import api from '../api'

const messages = ref([{ role: 'ai', text: '你好！我是校园智能助手，可以问我校园办事、报修、奖助等问题。' }])
const input = ref('')
const loading = ref(false)
const msgBox = ref(null)

async function scrollBottom() {
  await nextTick()
  if (msgBox.value) msgBox.value.scrollTop = msgBox.value.scrollHeight
}

async function send() {
  const q = input.value.trim()
  if (!q) return
  messages.value.push({ role: 'user', text: q })
  input.value = ''
  loading.value = true
  scrollBottom()
  try {
    const data = await api.post('/ai/chat', { question: q })
    messages.value.push({ role: 'ai', text: data.answer })
  } catch {
    messages.value.push({ role: 'ai', text: 'AI 助手暂时不可用，请稍后再试。' })
  } finally {
    loading.value = false
    scrollBottom()
  }
}

onMounted(async () => {
  try {
    const history = await api.get('/ai/history')
    history.forEach(h => {
      messages.value.push({ role: 'user', text: h.question })
      messages.value.push({ role: 'ai', text: h.answer })
    })
    scrollBottom()
  } catch { /* 忽略历史加载失败 */ }
})
</script>

<style scoped>
.chat-page { display: flex; flex-direction: column; height: 100vh; }
.messages { flex: 1; overflow-y: auto; padding: 12px; }
.msg { display: flex; margin-bottom: 12px; }
.msg.user { justify-content: flex-end; }
.bubble { max-width: 78%; padding: 10px 14px; border-radius: 12px; line-height: 1.6; white-space: pre-wrap; font-size: 14px; }
.msg.ai .bubble { background: #fff; border-top-left-radius: 2px; }
.msg.user .bubble { background: #1989fa; color: #fff; border-top-right-radius: 2px; }
.input-bar { display: flex; gap: 8px; padding: 8px 12px; background: #fff; align-items: center; }
.input-bar .van-field { flex: 1; background: #f2f3f5; border-radius: 20px; padding: 6px 16px; }
</style>
