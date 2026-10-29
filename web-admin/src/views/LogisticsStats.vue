<template>
  <div v-loading="loading">
    <el-row :gutter="16">
      <el-col :span="6"><el-card shadow="never" class="stat"><div class="num">{{ data.total }}</div><div class="lbl">工单总数</div></el-card></el-col>
      <el-col :span="6"><el-card shadow="never" class="stat"><div class="num warn">{{ data.pending }}</div><div class="lbl">待接单</div></el-card></el-col>
      <el-col :span="6"><el-card shadow="never" class="stat"><div class="num primary">{{ data.processing }}</div><div class="lbl">处理中</div></el-card></el-col>
      <el-col :span="6"><el-card shadow="never" class="stat"><div class="num ok">{{ data.done }}</div><div class="lbl">已完工</div></el-card></el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top:16px">
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>工单完成率</template>
          <el-progress type="dashboard" :percentage="data.completionRate" :width="160" :stroke-width="14" color="#67c23a" />
          <div class="center">已完成 {{ data.done }} / 共 {{ data.total }} 单</div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>服务口碑</template>
          <div class="rate">
            <div class="big">{{ data.avgRating }}</div>
            <div>
              <el-rate :model-value="Math.round(data.avgRating)" disabled show-score score-template="{value} 分" />
              <div class="center2">共 {{ data.reviewCount }} 条学生评价</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card shadow="never" style="margin-top:16px">
      <template #header>报修类型分布</template>
      <div v-for="c in data.byCategory" :key="c.name" class="bar-row">
        <span class="bar-label">{{ c.name }}</span>
        <el-progress :percentage="pct(c.count)" :stroke-width="18" :show-text="false" style="flex:1" />
        <span class="bar-val">{{ c.count }} 单</span>
      </div>
      <el-empty v-if="!data.byCategory?.length" description="暂无数据" :image-size="60" />
    </el-card>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import api from '../api'

const loading = ref(false)
const data = reactive({ total: 0, pending: 0, processing: 0, done: 0, completionRate: 0, avgRating: 0, reviewCount: 0, byCategory: [] })

const pct = n => {
  const max = Math.max(...data.byCategory.map(x => x.count), 1)
  return Math.round(n * 100 / max)
}
async function load() {
  loading.value = true
  try { Object.assign(data, await api.get('/stats/logistics')) } finally { loading.value = false }
}
onMounted(load)
</script>

<style scoped>
.stat { text-align: center; }
.num { font-size: 30px; font-weight: 600; color: #303133; }
.num.warn { color: #e6a23c; }
.num.primary { color: #409eff; }
.num.ok { color: #67c23a; }
.lbl { color: #909399; margin-top: 6px; }
.center { text-align: center; color: #909399; margin-top: -30px; }
.center2 { color: #909399; font-size: 13px; margin-top: 6px; }
.rate { display: flex; align-items: center; gap: 30px; justify-content: center; padding: 20px 0; }
.big { font-size: 46px; font-weight: 700; color: #e6a23c; }
.bar-row { display: flex; align-items: center; gap: 12px; margin-bottom: 14px; }
.bar-label { width: 60px; color: #606266; }
.bar-val { width: 60px; color: #909399; font-size: 13px; text-align: right; }
</style>
