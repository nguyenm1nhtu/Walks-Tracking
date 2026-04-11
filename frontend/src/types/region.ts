export interface RegionDto {
  id: string;
  name: string;
  code: string;
  regionImageUrl?: string | null;
}

export interface AddRegionDto {
  name: string;
  code: string;
  regionImageUrl?: string | null;
}

export interface UpdateRegionDto {
  name: string;
  code: string;
  regionImageUrl?: string | null;
}

export interface RegionQueryParams {
  filterOn?: string;
  filterQuery?: string;
  sortBy?: string;
  isAscending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
