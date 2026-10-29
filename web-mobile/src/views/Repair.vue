<template>
  <div>
    <van-nav-bar title="宿舍报修" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="tab">
      <van-tab title="提交报修">
        <van-form @submit="submit" style="margin-top:12px">
          <van-cell-group inset>
            <van-field v-model="form.dormitory" label="报修地点" placeholder="如：东区3栋502" required />
            <van-field v-model="form.contact" label="联系电话" placeholder="便于师傅联系（选填）" />
            <van-field v-model="form.description" label="故障描述" type="textarea" rows="3" autosize required
              placeholder="描述故障现象，提交时 AI 会自动分类并给出建议" />
            <van-field name="images" label="现场照片">
              <template #input>
                <van-uploader v-model="fileList" :after-read="onUpload" :max-count="3" />
              </template>
            </van-field>
          </van-cell-group>
          <div style="margin:20px 16px">
            <van-button round block type="primary" native-type="submit" :loading="submitting">提交（AI 智能分类）</van-button>
          </div>
        </van-form>
        <van-cell-group v-if="aiResult" title="AI 分析结果" inset>
          <van-cell title="故障分类" :value="aiResult.category" />
          <van-cell title="紧急程度">
            <template #value>
              <van-tag :type="aiResult.priority === '高' ? 'danger' : 'warning'">{{ aiResult.priority }}</van-tag>
            </template>
          </van-cell>
          <van-cell title="处理建议" :label="aiResult.aiAdvice" />
        </van-cell-group>
      </van-tab>

      <van-tab title="我的报修">
        <van-list style="padding:12px">
          <van-card v-for="r in list" :key="r.id" :title="`${r.category} · ${r.dormitory}`" :desc="r.description">
            <template #tags>
              <van-tag :type="stType(r.status)" style="margin-right:6px">{{ stName(r.status) }}</van-tag>
              <van-tag v-if="r.priority === '高'" type="danger" plain>紧急</van-tag>
              <van-tag v-if="r.rating" type="warning" plain>{{ r.rating }}星</van-tag>
            </template>
            <template #footer>
              <van-button size="mini" plain type="primary" @click="showProgress(r)">查看进度</van-button>
              <van-button v-if="r.status === 'done' && !r.rating" size="mini" type="success" @click="openReview(r)">评价</van-button>
            </template>
          </van-card>
          <van-empty v-if="!list.length" description="暂无报修记录" />
        </van-list>
      </van-tab>
    </van-tabs>

    <van-popup v-model:show="showP" position="bottom" round :style="{ height: '70%', padding: '16px', overflowY: 'auto' }">
      <h3>工单进度</h3>
      <van-steps direction="vertical" :active="progress.length - 1">
        <van-step v-for="p in progress" :key="p.id">
          <h4>{{ stName(p.status) }}</h4>
          <p>{{ p.remark || '—' }}</p>
          <p class="time">{{ p.operatorName }} · {{ fmt(p.createTime) }}</p>
          <van-image v-if="p.images" :src="(p.images || '').split(',')[0]" width="90" radius="6" />
        </van-step>
      </van-steps>
      <van-empty v-if="!progress.length" description="暂无进度" />
    </van-popup>

    <van-dialog v-model:show="showR" title="服务评价" show-cancel-button :before-close="submitReview">
      <div style="padding:16px;text-align:center">
        <van-rate v-model="review.rating" />
        <van-field v-model="review.comment" type="textarea" rows="2" placeholder="补充评价（选填）" style="margin-top:13px" />
      </div>
    </van-dialog>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref, watch } from 'vue'
import { showToast } from 'vant'
import api from '../api'

const tab = ref(0)
const form = reactive({ dormitory: '', description: '', contact: '', images: '' })
const fileList = ref([])
const submitting = ref(false)
const aiResult = ref(null)
const list = ref([])
const showP = ref(false)
const showR = ref(false)
const progress = ref([])
const review = reactive({ id: null, rating: 5, comment: '' })

const stName = s => ({ pending: '待处理', processing: '处理中', done: '已完成', rejected: '已驳回' }[s] || s)
const stType = s => ({ pending: 'danger', processing: 'warning', done: 'success', rejected: 'default' }[s] || 'default')
const fmt = t => t ? new Date(t).toLocaleString('zh-CN') : ''

async function onUpload(file) {
  const fd = new FormData()
  fd.append('file', file.file)
  const d = await api.post('/upload', fd)
  form.images = form.images ? form.images + ',' + d.url : d.url
  showToast('照片已上传')
}
async function submit() {
  submitting.value = true
  try {
    aiResult.value = await api.post('/repairs', form)
    showToast({ type: 'success', message: '提交成功，AI 已完成分类' })
    form.description = ''; form.images = ''; fileList.value = []
    loadList()
  } finally { submitting.value = false }
}
async function loadList() { list.value = await api.get('/repairs') }
async function showProgress(r) {
  progress.value = await api.get(`/repairs/${r.id}/progress`)
  showP.value = true
}
function openReview(r) { review.id = r.id; review.rating = 5; review.comment = ''; showR.value = true }
async function submitReview(action) {
  if (action !== 'confirm') return true
  await api.post(`/repairs/${review.id}/review`, { rating: review.rating, comment: review.comment })
  showToast('评价成功'); loadList(); return true
}

watch(tab, v => { if (v === 1) loadList() })
onMounted(loadList)
</script>

<style scoped>
.time { color: #969799; font-size: 12px; }
</style>
