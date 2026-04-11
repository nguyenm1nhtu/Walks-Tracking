export interface ImageDto {
  id: string
  fileName: string
  fileDescription?: string | null
  fileExtension: string
  fileSizeInBytes: number
  filePath: string
}

export interface ImageUploadRequestDto {
  file: File
  fileName: string
  fileDescription?: string | null
}
