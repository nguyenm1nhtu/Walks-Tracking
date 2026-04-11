import axios from 'axios'

const tokenStorageKey = 'jwtToken'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken()

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

export function getAccessToken(): string {
  return localStorage.getItem(tokenStorageKey) ?? ''
}

export function setAccessToken(token: string): void {
  const normalizedToken = token.trim().replace(/^Bearer\s+/i, '')

  if (normalizedToken) {
    localStorage.setItem(tokenStorageKey, normalizedToken)
    return
  }

  clearAccessToken()
}

export function clearAccessToken(): void {
  localStorage.removeItem(tokenStorageKey)
}

export default apiClient
