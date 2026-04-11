export interface RegisterRequestDto {
  username: string
  password: string
  roles?: string[]
}

export interface LoginRequestDto {
  username: string
  password: string
}

export interface LoginResponseDto {
  jwtToken: string
}
