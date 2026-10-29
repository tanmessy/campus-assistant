import axios from 'axios'
import { ElMessage } from 'element-plus'
import router from './router'

const api = axios.create({ baseURL: '/api', timeout: 20000 })

// 请求头自动携带 JWT
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// 统一错误提示
api.interceptors.response.use(
  resp => {
    const body = resp.data
    if (body.code !== 200) {
      ElMessage.error(body.message || '请求失败')
      if (body.code === 401) { localStorage.clear(); router.push('/login') }
      return Promise.reject(new Error(body.message))
    }
    return body.data
  },
  err => {
    ElMessage.error(err.response?.data?.message || '网络异常')
    if (err.response?.status === 401) { localStorage.clear(); router.push('/login') }
    return Promise.reject(err)
  }
)

export default api
