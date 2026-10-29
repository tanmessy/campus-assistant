<template>
  <el-tabs v-model="tab">
    <el-tab-pane label="食堂菜单" name="cafeteria">
      <el-card shadow="never">
        <template #header>
          <div class="hd">
            <span>食堂菜单维护</span>
            <el-button type="primary" :icon="Plus" @click="openCaf()">新增菜品</el-button>
          </div>
        </template>
        <el-table :data="menus" stripe v-loading="loading">
          <el-table-column prop="canteen" label="食堂" width="110" />
          <el-table-column prop="window" label="窗口" width="120" />
          <el-table-column prop="mealType" label="餐次" width="90" />
          <el-table-column prop="dishName" label="菜品" min-width="150" />
          <el-table-column prop="price" label="价格(元)" width="100" />
          <el-table-column prop="description" label="描述" min-width="140" show-overflow-tooltip />
          <el-table-column label="操作" width="140">
            <template #default="{ row }">
              <el-button link type="primary" @click="openCaf(row)">编辑</el-button>
              <el-button link type="danger" @click="delCaf(row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </el-tab-pane>

    <el-tab-pane label="场地资源" name="venue">
      <el-card shadow="never">
        <template #header>
          <div class="hd">
            <span>场地资源维护</span>
            <el-button type="primary" :icon="Plus" @click="openVenue()">新增场地</el-button>
          </div>
        </template>
        <el-table :data="venues" stripe v-loading="loading">
          <el-table-column prop="name" label="场地名称" min-width="140" />
          <el-table-column prop="location" label="位置" min-width="140" />
          <el-table-column prop="capacity" label="容纳人数" width="100" />
          <el-table-column prop="equipment" label="设备" min-width="180" show-overflow-tooltip />
          <el-table-column label="操作" width="100">
            <template #default="{ row }">
              <el-button link type="primary" @click="openVenue(row)">编辑</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </el-tab-pane>
  </el-tabs>

  <el-dialog v-model="cafDialog" :title="cafForm.id ? '编辑菜品' : '新增菜品'" width="480px">
    <el-form :model="cafForm" label-width="80px">
      <el-form-item label="食堂"><el-input v-model="cafForm.canteen" placeholder="如：一食堂" /></el-form-item>
      <el-form-item label="窗口"><el-input v-model="cafForm.window" placeholder="如：家常热菜" /></el-form-item>
      <el-form-item label="餐次">
        <el-select v-model="cafForm.mealType">
          <el-option label="早餐" value="早餐" /><el-option label="午餐" value="午餐" /><el-option label="晚餐" value="晚餐" />
        </el-select>
      </el-form-item>
      <el-form-item label="菜品名"><el-input v-model="cafForm.dishName" /></el-form-item>
      <el-form-item label="价格"><el-input-number v-model="cafForm.price" :min="0" :precision="1" /></el-form-item>
      <el-form-item label="描述"><el-input v-model="cafForm.description" /></el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="cafDialog = false">取消</el-button>
      <el-button type="primary" :loading="saving" @click="saveCaf">保存</el-button>
    </template>
  </el-dialog>

  <el-dialog v-model="venueDialog" :title="venueForm.id ? '编辑场地' : '新增场地'" width="480px">
    <el-form :model="venueForm" label-width="80px">
      <el-form-item label="名称"><el-input v-model="venueForm.name" /></el-form-item>
      <el-form-item label="位置"><el-input v-model="venueForm.location" /></el-form-item>
      <el-form-item label="容量"><el-input-number v-model="venueForm.capacity" :min="1" /></el-form-item>
      <el-form-item label="设备"><el-input v-model="venueForm.equipment" /></el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="venueDialog = false">取消</el-button>
      <el-button type="primary" :loading="saving" @click="saveVenue">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import api from '../api'

const tab = ref('cafeteria')
const loading = ref(false)
const saving = ref(false)
const menus = ref([])
const venues = ref([])
const cafDialog = ref(false)
const venueDialog = ref(false)
const cafForm = reactive({ id: null, canteen: '一食堂', window: '', mealType: '午餐', dishName: '', price: 0, description: '' })
const venueForm = reactive({ id: null, name: '', location: '', capacity: 30, equipment: '' })

async function load() {
  loading.value = true
  try {
    menus.value = await api.get('/cafeteria')
    venues.value = await api.get('/venues')
  } finally { loading.value = false }
}
function openCaf(row) {
  if (row) Object.assign(cafForm, row)
  else Object.assign(cafForm, { id: null, canteen: '一食堂', window: '', mealType: '午餐', dishName: '', price: 0, description: '' })
  cafDialog.value = true
}
async function saveCaf() {
  if (!cafForm.dishName) return ElMessage.warning('请填写菜品名称')
  saving.value = true
  try {
    const body = { canteen: cafForm.canteen, window: cafForm.window, mealType: cafForm.mealType, dishName: cafForm.dishName, price: cafForm.price, description: cafForm.description }
    if (cafForm.id) await api.put(`/cafeteria/${cafForm.id}`, body)
    else await api.post('/cafeteria', body)
    ElMessage.success('已保存'); cafDialog.value = false; load()
  } finally { saving.value = false }
}
async function delCaf(row) {
  await ElMessageBox.confirm(`删除菜品「${row.dishName}」？`, '提示', { type: 'warning' })
  await api.delete(`/cafeteria/${row.id}`); ElMessage.success('已删除'); load()
}
function openVenue(row) {
  if (row) Object.assign(venueForm, row)
  else Object.assign(venueForm, { id: null, name: '', location: '', capacity: 30, equipment: '' })
  venueDialog.value = true
}
async function saveVenue() {
  if (!venueForm.name) return ElMessage.warning('请填写场地名称')
  saving.value = true
  try {
    await api.post('/venues', { name: venueForm.name, location: venueForm.location, capacity: venueForm.capacity, equipment: venueForm.equipment })
    ElMessage.success('已保存'); venueDialog.value = false; load()
  } finally { saving.value = false }
}
onMounted(load)
</script>

<style scoped>
.hd { display: flex; justify-content: space-between; align-items: center; }
</style>
