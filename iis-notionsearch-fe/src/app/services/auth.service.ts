import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, StandardResponse } from '../models/models';
import { parseJwt } from '../utils/jwt-utils';
import { API_URL } from '../tokens/api-url.token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${inject(API_URL)}/api/auth`;

  private _accessToken: string | null = null;

  readonly isAuthenticated = signal<boolean>(false);
  readonly isAdmin = signal<boolean>(false);
  readonly username = signal<string | null>(null);

  login(request: LoginRequest): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/login`, request, { withCredentials: true })
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  register(request: RegisterRequest): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/register`, request, { withCredentials: true })
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  refresh(): Observable<StandardResponse<AuthResponse>> {
    return this.http
      .post<StandardResponse<AuthResponse>>(`${this.baseUrl}/refresh`, {}, { withCredentials: true })
      .pipe(tap((res) => this.handleAuthResponse(res)));
  }

  signOut(): void {
    this.clearToken();
    this.http
      .post<StandardResponse<boolean>>(`${this.baseUrl}/signout`, {}, { withCredentials: true })
      .subscribe();
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return this._accessToken;
  }

  isTokenExpired(): boolean {
    if (!this._accessToken) return true;
    const payload = parseJwt(this._accessToken);
    if (!payload?.['exp']) return true;
    return (payload['exp'] as number - 30) * 1000 < Date.now();
  }

  private handleAuthResponse(res: StandardResponse<AuthResponse>): void {
    if (!res.success || !res.data) return;

    const token = res.data.accessToken;
    this._accessToken = token;
    const payload = parseJwt(token);
    const role = (payload?.['role'] as string);
    const user = (payload?.['unique_name'] as string);

    this.isAuthenticated.set(true);
    this.isAdmin.set(role === 'Admin');
    this.username.set(user);
  }

  handleUnauthorized(): void {
    this.refresh().subscribe({
      error: () => {
        this.clearToken();
        this.router.navigate(['/login']);
      }
    });
  }

  clearToken(): void {
    this._accessToken = null;
    this.isAuthenticated.set(false);
    this.isAdmin.set(false);
    this.username.set(null);
  }
}
