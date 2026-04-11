import apiClient from "../lib/apiClient";
import type {
  AddRegionDto,
  RegionDto,
  RegionQueryParams,
  UpdateRegionDto,
} from "../types/region";

export async function getRegions(
  params?: RegionQueryParams,
): Promise<RegionDto[]> {
  const response = await apiClient.get<RegionDto[]>("/Region", {
    params,
  });

  return response.data;
}

export async function getRegionById(id: string): Promise<RegionDto> {
  const response = await apiClient.get<RegionDto>(`/Region/${id}`);
  return response.data;
}

export async function createRegion(payload: AddRegionDto): Promise<RegionDto> {
  const response = await apiClient.post<RegionDto>("/Region", payload);
  return response.data;
}

export async function updateRegion(
  id: string,
  payload: UpdateRegionDto,
): Promise<RegionDto> {
  const response = await apiClient.put<RegionDto>(`/Region/${id}`, payload);
  return response.data;
}

export async function deleteRegion(id: string): Promise<void> {
  await apiClient.delete(`/Region/${id}`);
}
