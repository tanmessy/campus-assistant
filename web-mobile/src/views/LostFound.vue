<template>
  <div>
    <van-nav-bar title="失物招领" left-arrow @click-left="$router.back()" />
    <div class="bar">
      <van-tabs v-model:active="type" @change="load" style="flex:1">
        <van-tab title="全部" name="" />
        <van-tab title="寻物" name="lost" />
        <van-tab title="招领" name="found" />
      </van-tabs>
      <van-button size="small" type="primary" icon="plus" style="margin:8px" @click="show = true">发布</van-button>
    </div>

    <van-list style="padding:12px">
      <van-card v-for="x in list" :key="x.id" :title="x.title" :desc="x.description">
        <template #tags>
          <van-tag :type="x.type === 'lost' ? 'danger' : 'success'">{{ x.type === 'lost' ? '寻物' : '招领' }}</van-tag>
          <van-tag plain style="margin-left:6px">{{ x.location }}</van-tag>
        </template>
        <template #footer>
          <span style="color:#969799;font-size:12px">{{ x.publisher }} · {{ x.contact }}</span>
        </template>
      </van-card>
      <van-empty v-if="!list.length" description="暂无信息" />
    </van-list>

    <van-popup v-model:show="show" position="bottom" round style="padding:20px 0 30px">
      <van-form @submit="publish">
        <van-cell-group inset>
          <van-field label="类型">
            <template #input>
              <van-radio-group v-model="form.type" direction="horizontal">
                <van-radio name="lost">寻物</van-radio><van-radio name="found">招领</van-radio>
              </van-radio-group>
            </template>
          </van-field>
          <van-field v-model="form.title" label="标题" placeholder="如：黑色华为耳机" required />
          <van-field v-model="form.description" label="描述" type="textarea" rows="2" autosize placeholder="特征、时间等" />
          <van-field v-model="form.location" label="地点" placeholder="丢失/拾获地点" />
          <van-field v-model="form.contact" label="联系方式" placeholder="手机号或微信号" />
        </van-cell-group>
        <div style="margin:20px 16px">
          <van-button round block type="primary" native-type="submit" :loading="publishing">发布（AI 自动匹配）</van-button>
        </div>
      </van-form>
    </van-popup>

    <van-dialog v-model:show="showMatches" title="AI 为你找到可能相关的信息">
      <van-cell v-for="m in matches" :key="m.id" :title="m.title" :label="`${m.location || ''} · 联系：${m.contact || '暂无'}`"
        :value="`相似度 ${Math.round(m.similarity * 100)}%`" />
      <van-empty v-if="!matches.length" description="暂未找到匹配信息" />
    </van-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { showToast } from 'vant'
import api from '../api'

const type = ref('')
const list = ref([])
const show = ref(false)
const publishing = ref(false)
const showMatches = ref(false)
const matches = ref([])
const form = reactive({ type: 'lost', title: '', description: '', location: '', contact: '' })

async function load() { list.value = await api.get('/lostfound', { params: { type: type.value } }) }

async function publish() {
  publishing.value = true
  try {
    const data = await api.post('/lostfound', form)
    show.value = false
    matches.value = data.matches
    showMatches.value = true   // AI 语义匹配结果弹窗
    showToast({ type: 'success', message: '发布成功' })
    load()
  } finally { publishing.value = false }
}

onMounted(load)
</script>

<style scoped>
.bar { display: flex; align-items: center; background: #fff; }
</style>
