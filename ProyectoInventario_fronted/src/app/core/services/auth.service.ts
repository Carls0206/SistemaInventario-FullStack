import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_URL } from '../config/api.config';
import { LoginRequest, LoginResponse, RegisterRequest, UsuarioDto } from '../models/auth.model';

const TOKEN_KEY = 'access_token';
const USUARIO_KEY = 'usuario_actual';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_URL}/auth/login`, credentials)
      .pipe(
        tap((res) => {
          this.setToken(res.token);
          this.setUsuario(res.usuario);
        }),
      );
  }

  register(data: RegisterRequest): Observable<UsuarioDto> {
    return this.http.post<UsuarioDto>(`${API_URL}/auth/register`, data);
  }

  logout(): void {
    this.removeToken();
    this.removeUsuario();
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(TOKEN_KEY);
  }

  getUsuario(): UsuarioDto | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    const raw = localStorage.getItem(USUARIO_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as UsuarioDto;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private setToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(TOKEN_KEY, token);
    }
  }

  private setUsuario(usuario: UsuarioDto): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(USUARIO_KEY, JSON.stringify(usuario));
    }
  }

  private removeToken(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(TOKEN_KEY);
    }
  }

  private removeUsuario(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(USUARIO_KEY);
    }
  }
}
