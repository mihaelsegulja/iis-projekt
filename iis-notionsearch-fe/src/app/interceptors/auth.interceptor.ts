import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

const SKIP = ['api/auth/login', 'api/auth/register', 'api/auth/refresh', 'api/auth/signout'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (SKIP.some((p) => req.url.includes(p))) return next(req);

  const auth = inject(AuthService);
  const token = auth.getAccessToken();
  if (!token) return next(req);

  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
