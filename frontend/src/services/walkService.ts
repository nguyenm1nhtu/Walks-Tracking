import apiClient from '../lib/apiClient'
import type { AddWalkDto, UpdateWalkDto, WalkDto, WalkQueryParams } from '../types/walk'

export async function getWalks(params?: WalkQueryParams): Promise<WalkDto[]> {
  const response = await apiClient.get<WalkDto[]>('/Walk', { params })
  return response.data
}

export async function getWalkById(id: string): Promise<WalkDto> {
  const response = await apiClient.get<WalkDto>(`/Walk/${id}`)
  return response.data
}

export async function createWalk(payload: AddWalkDto): Promise<WalkDto> {
  const response = await apiClient.post<WalkDto>('/Walk', payload)
  return response.data
}

export async function updateWalk(id: string, payload: UpdateWalkDto): Promise<WalkDto> {
  const response = await apiClient.put<WalkDto>(`/Walk/${id}`, payload)
  return response.data
}

export async function deleteWalk(id: string): Promise<void> {
  await apiClient.delete(`/Walk/${id}`)
}
