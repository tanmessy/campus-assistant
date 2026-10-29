<template>
  <div>
    <van-nav-bar title="工单详情" left-arrow @click-left="$router.back()" />
    <div v-if="order.id" style="padding-bottom:70px">
      <van-cell-group inset title="基本信息">
        <van-cell title="单号" :value="'#' + order.id" />
        <van-cell title="报修人" :value="order.studentName" />
        <van-cell title="联系电话" :value="order.contact || '未填写'" />
        <van-cell title="报修地点" :value="order.dormitory" />
        <van-cell title="故障描述" :label="order.description" />
        <van-cell title="AI 分类/紧急度">
          <template #value>
            <van-tag type="primary">{{ order.category }}</van-tag>
            <van-tag :type="order.priority === '高' ? 'danger' : 'warning'" style="margin-left:6px">{{ order.priority }}</van-tag>
          </template>
        </van-cell>
        <van-cell title="AI 维修建议" :label="order.aiAdvice" />
        <van-cell title="当前状态" :value="stName(order.status)" />
      </van-cell-group>

      <van-cell-group inset title="进度时间轴" style="margin-top:12px">
        <van-steps direction="vertical" :active="order.progress ? order.progress.length - 1 : 0">
          <van-step v-for="p in order.progress" :key="p.id">
            <h4>{{ stName(p.status) }}</h4>
            <p>{{ p.remark || '—' }}</p>
            <p class="time">{{ p.operatorName }} · {{ fmt(p.createTime) }}</p>
            <van-image v-if="p.images" :src="firstImg(p.images)" width="90" radius="6" />
          </van-step>
        </van-steps>
      </van-cell-group>
    </div>

    <van-action-bar v-if="order.id">
      <van-button v-if="order.status === 'pending'" type="primary" block round style="flex:1" :loading="saving" @click="act('accept')">接单</van-button>
      <template v-else-if="order.status === 'processing'">
        <van-button plain type="primary" round style="flex:1" @click="showProgress = true">更新进度</van-button>
        <van-button type="success" round style="flex:1" :loading="saving" @click="act('done')">完工确认</van-button>
      </template>
      <van-tag v-else-if="order.status === 'done'" type="success" size="large">该工单已完工</van-tag>
    </van-action-bar>

    <van-popup v-model:show="showProgress" position="bottom" round :style="{ padding: '16px' }">
      <h4>更新维修进度</h4>
      <van-field v-model="progressForm.remark" type="textarea" rows="3" placeholder="进度说明，如：已更换配件，正在测试" />
      <div class="upload-row">
        <van-uploader :after-read="onUpload" :max-count="1">
          <van-button icon="photograph" size="small" plain type="primary">上传现场照片</van-button>
        </van-uploader>
        <span v-if="progressForm.images" class="ok">已上传</span>
      </div>
      <van-button type="primary" block round style="margin-top:12px" :loading="saving" @click="submitProgress">提交进度</van-button>
    </van-popup>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { showToast } from 'vant'
import api from '../../api'

const route = useRoute()
const router = useRouter()
const order = ref({})
const saving = ref(false)
const showProgress = ref(false)
const progressForm = reactive({ remark: '', images: '' })
const stName = s => ({ pending: '待接单', processing: '处理中', done: '已完工', rejected: '已驳回' }[s] || s)
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''
const firstImg = s => (s || '').split(',')[0]

async function load() { order.value = await api.get(`/repairs/${route.params.id}`) }

async function act(action) {
  saving.value = true
  try {
    await api.post(`/repairs/${order.value.id}/handle`, { action })
    showToast(action === 'accept' ? '已接单' : '已完工')
    await load()
  } finally { saving.value = false }
}
async function onUpload(file) {
  const fd = new FormData()
  fd.append('file', file.file)
  const d = await api.post('/upload', fd)
  progressForm.images = d.url
  showToast('照片已上传')
}
async function submitProgress() {
  saving.value = true
  try {
    await api.post(`/repairs/${order.value.id}/progress`, { status: 'processing', remark: progressForm.remark, images: progressForm.images })
    showToast('进度已更新'); showProgress.value = false; progressForm.remark = ''; progressForm.images = ''
    await load()
  } finally { saving.value = false }
}
onMounted(load)
</script>

<style scoped>
.time { color: #969799; font-size: 12px; }
.upload-row { display: flex; align-items: center; gap: 12px; margin-top: 12px; }
.ok { color: #07c160; font-size: 13px; }
</style>
