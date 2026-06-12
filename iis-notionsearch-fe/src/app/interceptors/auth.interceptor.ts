import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/api/auth/')) {
    return next(req);
  }

  const auth = inject(AuthService);
  const token = auth.getAccessToken();
  
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req).pipe(
    catchError((err) => {
      if (err.status !== 401 || req.headers.has('X-Auth-Retry')) {
        return throwError(() => err);
      }

      return auth.refresh().pipe(
        switchMap(() => {
          const newToken = auth.getAccessToken();
          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${newToken ?? ''}`,
              'X-Auth-Retry': 'true',
            },
          });
          return next(retryReq);
        }),
        catchError(() => {
          auth.clearToken();
          return throwError(() => err);
        }),
      );
    }),
  );
};
