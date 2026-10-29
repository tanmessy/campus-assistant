<template>
  <div>
    <van-nav-bar title="场地预约" left-arrow @click-left="$router.back()" />
    <van-tabs v-model:active="tab">
      <van-tab title="预约场地">
        <van-list style="padding:12px">
          <van-card v-for="v in venues" :key="v.id" :title="v.name" :desc="`${v.location} · 容纳 ${v.capacity} 人 · ${v.equipment || '无设备'}`">
            <template #footer>
              <van-button size="small" type="primary" @click="openBook(v)">预约</van-button>
            </template>
          </van-card>
        </van-list>
      </van-tab>
      <van-tab title="我的预约">
        <van-list style="padding:12px">
          <van-cell-group inset v-for="b in bookings" :key="b.id" style="margin-bottom:10px">
            <van-cell :title="b.venueName" :label="`${fmtDT(b.startTime)} ~ ${fmtDT(b.endTime)}`">
              <template #value>
                <van-tag :type="{ pending: 'warning', approved: 'success', rejected: 'danger' }[b.status]">
                  {{ { pending: '待审核', approved: '已通过', rejected: '已拒绝' }[b.status] }}
                </van-tag>
              </template>
            </van-cell>
          </van-cell-group>
          <van-empty v-if="!bookings.length" description="暂无预约" />
        </van-list>
      </van-tab>
    </van-tabs>

    <van-popup v-model:show="show" position="bottom" round style="padding:20px 0 30px">
      <van-form @submit="book">
        <van-cell-group inset>
          <van-cell title="场地" :value="current?.name" />
          <van-field v-model="startTime" is-link readonly label="开始时间" placeholder="选择时间" @click="openPicker('start')" required />
          <van-field v-model="endTime" is-link readonly label="结束时间" placeholder="选择时间" @click="openPicker('end')" required />
          <van-field v-model="reason" label="预约事由" placeholder="如：社团活动" required />
        </van-cell-group>
        <div style="margin:20px 16px">
          <van-button round block type="primary" native-type="submit">提交预约</van-button>
        </div>
      </van-form>
    </van-popup>

    <van-popup v-model:show="showDate" position="bottom" round>
      <van-date-picker v-model="dateValue" title="选择日期" @confirm="onDate" @cancel="showDate = false" />
    </van-popup>
    <van-popup v-model:show="showTime" position="bottom" round>
      <van-time-picker v-model="timeValue" title="选择时间" @confirm="onTime" @cancel="showTime = false" />
    </van-popup>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { showToast } from 'vant'
import api from '../api'

const tab = ref(0)
const venues = ref([])
const bookings = ref([])
const show = ref(false)
const current = ref(null)
const startTime = ref('')
const endTime = ref('')
const reason = ref('')

// 日期 + 时间两步选择：先选日期，再选时间
const picking = ref('')
const showDate = ref(false)
const showTime = ref(false)
const today = new Date()
const dateValue = ref([String(today.getFullYear()), String(today.getMonth() + 1).padStart(2, '0'), String(today.getDate()).padStart(2, '0')])
const timeValue = ref(['09', '00'])
const pendingDate = ref('')

function openPicker(which) { picking.value = which; showDate.value = true }
function onDate({ selectedValues }) { pendingDate.value = selectedValues.join('-'); showDate.value = false; showTime.value = true }
function onTime({ selectedValues }) {
  const v = `${pendingDate.value} ${selectedValues[0]}:${selectedValues[1]}`
  if (picking.value === 'start') startTime.value = v; else endTime.value = v
  showTime.value = false
}

function openBook(v) { current.value = v; show.value = true }

async function book() {
  if (!startTime.value || !endTime.value) return showToast('请选择时间段')
  if (endTime.value <= startTime.value) return showToast('结束时间需晚于开始时间')
  await api.post('/venues/bookings', {
    venueId: current.value.id,
    startTime: startTime.value.replace(' ', 'T') + ':00',
    endTime: endTime.value.replace(' ', 'T') + ':00',
    reason: reason.value
  })
  show.value = false
  showToast({ type: 'success', message: '已提交，等待审核' })
  loadBookings()
}

function fmtDT(s) { return (s || '').replace('T', ' ').slice(0, 16) }

async function loadBookings() { bookings.value = await api.get('/venues/bookings') }

watch(tab, v => { if (v === 1) loadBookings() })
onMounted(async () => { venues.value = await api.get('/venues'); loadBookings() })
</script>
