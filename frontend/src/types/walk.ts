import type { RegionDto } from './region'

export interface DifficultyDto {
  id: string
  name: string
}

export interface WalkDto {
  id: string
  name: string
  description: string
  lengthInKm: number
  walkImageUrl?: string | null
  region: RegionDto
  difficulty: DifficultyDto
}

export interface AddWalkDto {
  name: string
  description: string
  lengthInKm: number
  walkImageUrl?: string | null
  difficultyId: string
  regionId: string
}

export interface UpdateWalkDto {
  name: string
  description: string
  lengthInKm: number
  walkImageUrl?: string | null
  difficultyId: string
  regionId: string
}

export interface WalkQueryParams {
  filterOn?: string
  filterQuery?: string
  sortBy?: string
  isAscending?: boolean
  pageNumber?: number
  pageSize?: number
}

export interface WalkFormValues {
  name: string
  description: string
  lengthInKm: string
  walkImageUrl: string
  difficultyId: string
  regionId: string
}
