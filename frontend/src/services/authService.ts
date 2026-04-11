import apiClient, { setAccessToken } from '../lib/apiClient'
import type {
  LoginRequestDto,
  LoginResponseDto,
  RegisterRequestDto,
} from '../types/auth'

export async function register(payload: RegisterRequestDto): Promise<string> {
  const response = await apiClient.post<string>('/Auth/Register', payload)
  return response.data
}

export async function login(payload: LoginRequestDto): Promise<LoginResponseDto> {
  const response = await apiClient.post<LoginResponseDto>('/Auth/Login', payload)
  setAccessToken(response.data.jwtToken)
  return response.data
}

export async function logout(): Promise<string> {
  const response = await apiClient.post<string>('/Auth/Logout')
  return response.data
}
