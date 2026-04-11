import apiClient from '../lib/apiClient'
import type { DifficultyDto } from '../types/walk'

export async function getDifficulties(): Promise<DifficultyDto[]> {
  const response = await apiClient.get<DifficultyDto[]>('/Difficulty')
  return response.data
}
