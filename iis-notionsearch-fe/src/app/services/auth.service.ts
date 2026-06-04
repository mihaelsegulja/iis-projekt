import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  StandardResponse,
} from '../models/models';
import { parseJwt } from '../utils/jwt-utils';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = 'api/auth';
  private readonly accessTokenKey = 'accessToken';

  readonly isAuthenticated = signal(false);
  readonly isAdmin = signal(false);
  readonly username = signal<string | null>(null);

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {
    this.restoreSession();
  }

  login(request: LoginRequest): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/login`, request)
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  register(request: RegisterRequest): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/register`, request)
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  refresh(): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/refresh`, {})
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  signOut(): Observable<StandardResponse<boolean>> {
    return this.http
      .post<StandardResponse<boolean>>(`${this.baseUrl}/signout`, {})
      .pipe(tap(() => {
        this.clearToken();
        this.router.navigate(['/login']);
      }));
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  private handleAuthResponse(res: StandardResponse<AuthResponse>): void {
    if (!res.success || !res.data) return;
    const token = res.data.accessToken;
    localStorage.setItem(this.accessTokenKey, token);
    this.applyToken(token);
  }

  private restoreSession(): void {
    const token = this.getAccessToken();
    if (!token) return;

    const payload = parseJwt(token);
    if (!payload || !payload['exp']) {
      this.clearToken();
      return;
    }

    if (Date.now() >= (payload['exp'] as number) * 1000) {
      this.clearToken();
      return;
    }

    this.applyToken(token);
  }

  private applyToken(token: string): void {
    const payload = parseJwt(token);
    this.isAuthenticated.set(true);
    this.isAdmin.set((payload?.['role'] as string) === 'Admin');
    this.username.set((payload?.['unique_name'] as string) ?? null);
  }

  private clearToken(): void {
    localStorage.removeItem(this.accessTokenKey);
    this.isAuthenticated.set(false);
    this.isAdmin.set(false);
    this.username.set(null);
  }
}
