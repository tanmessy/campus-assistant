<template>
  <div class="wrap">
    <el-card shadow="never" class="chat-card">
      <template #header>
        <div class="hd">
          <span>AI 校园智能助手（网页版）</span>
          <el-tag :type="aiEnabled ? 'success' : 'info'" size="small">
            {{ aiEnabled ? '大模型已接入' : '本地知识库模式' }}
          </el-tag>
        </div>
      </template>

      <div class="quick">
        <el-tag v-for="q in quicks" :key="q" class="q" @click="ask(q)">{{ q }}</el-tag>
      </div>

      <div class="msgs" ref="msgBox">
        <div v-for="(m, i) in messages" :key="i" :class="['msg', m.role]">
          <div class="avatar">{{ m.role === 'user' ? '我' : '🤖' }}</div>
          <div class="bubble">{{ m.text }}</div>
        </div>
        <div v-if="loading" class="msg ai"><div class="avatar">🤖</div><div class="bubble">正在思考…</div></div>
      </div>

      <div class="input">
        <el-input v-model="text" placeholder="输入你的问题，例如：校园卡丢了怎么办" @keyup.enter="send" />
        <el-button type="primary" :loading="loading" @click="send">发送</el-button>
      </div>
    </el-card>

    <el-card shadow="never" class="side">
      <template #header><span>向老师提问</span></template>
      <el-input v-model="consult.question" type="textarea" :rows="4" placeholder="描述你的问题，提交后会由老师回复" />
      <el-select v-model="consult.category" style="width:100%;margin-top:10px">
        <el-option label="学习" value="学习" />
        <el-option label="生活" value="生活" />
        <el-option label="心理" value="心理" />
        <el-option label="其他" value="其他" />
      </el-select>
      <el-button type="success" style="width:100%;margin-top:10px" :loading="cLoading" @click="submitConsult">提交给老师</el-button>

      <el-divider>我的咨询记录</el-divider>
      <div v-for="c in consults" :key="c.id" class="consult">
        <div class="cq">Q：{{ c.question }}</div>
        <div class="ca" v-if="c.reply">A（{{ c.replierName }}）：{{ c.reply }}</div>
        <el-tag v-else size="small" type="warning">待回复</el-tag>
      </div>
      <el-empty v-if="!consults.length" description="暂无咨询" :image-size="60" />
    </el-card>
  </div>
</template>

<script setup>
import { nextTick, onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import api from '../api'

const messages = ref([
  { role: 'ai', text: '你好！我是校园生活智能助手，可以帮你查询校园卡、请假、报修、奖助学金、图书馆等校园信息，也可以回答你的各种校园生活问题。' }
])
const text = ref('')
const loading = ref(false)
const aiEnabled = ref(false)
const msgBox = ref(null)
const consults = ref([])
const cLoading = ref(false)
const consult = reactive({ question: '', category: '学习' })
const quicks = ['校园卡丢了怎么办', '怎么申请请假', '图书馆借书规则', '奖学金怎么申请']

async function send() { if (text.value.trim()) await ask(text.value.trim()) }

async function ask(q) {
  messages.value.push({ role: 'user', text: q })
  text.value = ''
  loading.value = true
  await scroll()
  try {
    const history = messages.value.slice(0, -1).slice(-6).map(m => ({ role: m.role === 'user' ? 'user' : 'ai', text: m.text }))
    const d = await api.post('/ai/chat', { question: q, history })
    aiEnabled.value = d.aiEnabled
    messages.value.push({ role: 'ai', text: d.answer })
  } catch (e) {
    messages.value.push({ role: 'ai', text: '抱歉，服务暂时不可用，请稍后再试。' })
  } finally { loading.value = false; scroll() }
}
async function scroll() { await nextTick(); if (msgBox.value) msgBox.value.scrollTop = msgBox.value.scrollHeight }

async function submitConsult() {
  if (!consult.question.trim()) return ElMessage.warning('请输入问题内容')
  cLoading.value = true
  try {
    await api.post('/consultations', { question: consult.question, category: consult.category })
    ElMessage.success('已提交，老师会尽快回复')
    consult.question = ''
    loadConsults()
  } finally { cLoading.value = false }
}
async function loadConsults() { consults.value = await api.get('/consultations/my') }

onMounted(loadConsults)
</script>

<style scoped>
.wrap { display: flex; gap: 16px; align-items: flex-start; }
.chat-card { flex: 1; }
.side { width: 340px; }
.hd { display: flex; justify-content: space-between; align-items: center; }
.quick { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 12px; }
.q { cursor: pointer; }
.msgs { height: 420px; overflow-y: auto; padding: 8px 4px; background: #f7f9fc; border-radius: 8px; }
.msg { display: flex; gap: 10px; margin-bottom: 14px; }
.msg.user { flex-direction: row-reverse; }
.avatar { width: 32px; height: 32px; border-radius: 50%; background: #409eff; color: #fff; display: flex; align-items: center; justify-content: center; flex-shrink: 0; font-size: 14px; }
.msg.user .avatar { background: #67c23a; }
.bubble { max-width: 76%; padding: 10px 14px; border-radius: 10px; background: #fff; line-height: 1.7; white-space: pre-wrap; box-shadow: 0 1px 4px rgba(0,0,0,.06); }
.msg.user .bubble { background: #d9ecff; }
.input { display: flex; gap: 10px; margin-top: 12px; }
.consult { padding: 8px 0; border-bottom: 1px solid #f0f2f5; font-size: 13px; }
.cq { color: #303133; }
.ca { color: #67c23a; margin-top: 4px; }
</style>
