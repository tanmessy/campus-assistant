import axios from 'axios'
import { showToast } from 'vant'
import router from './router'

const api = axios.create({ baseURL: '/api', timeout: 20000 })

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  resp => {
    const body = resp.data
    if (body.code !== 200) {
      showToast({ type: 'fail', message: body.message || '请求失败' })
      if (body.code === 401) { localStorage.clear(); router.push('/login') }
      return Promise.reject(new Error(body.message))
    }
    return body.data
  },
  err => {
    showToast({ type: 'fail', message: err.response?.data?.message || '网络异常' })
    if (err.response?.status === 401) { localStorage.clear(); router.push('/login') }
    return Promise.reject(err)
  }
)

export default api
