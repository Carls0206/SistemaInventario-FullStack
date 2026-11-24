export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  usuario: UsuarioDto;
}

export interface UsuarioDto {
  id: number;
  nombre: string;
  email: string;
}

export interface RegisterRequest {
  nombre: string;
  email: string;
  password: string;
}