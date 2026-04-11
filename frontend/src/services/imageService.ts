import apiClient from '../lib/apiClient'
import type { ImageDto, ImageUploadRequestDto } from '../types/image'

export async function uploadImage(payload: ImageUploadRequestDto): Promise<ImageDto> {
  const formData = new FormData()
  formData.append('file', payload.file)
  formData.append('fileName', payload.fileName)

  if (payload.fileDescription?.trim()) {
    formData.append('fileDescription', payload.fileDescription.trim())
  }

  const response = await apiClient.post<ImageDto>('/Image/Upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  })

  return response.data
}
