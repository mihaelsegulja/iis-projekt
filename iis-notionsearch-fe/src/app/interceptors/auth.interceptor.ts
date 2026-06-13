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

  if (token && auth.isTokenExpired()) {
    return auth.refresh().pipe(
      switchMap(() => {
        const newToken = auth.getAccessToken();
        const reqWithToken = newToken
          ? req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } })
          : req;
        return next(reqWithToken);
      }),
      catchError(() => {
        auth.clearToken();
        return next(req);
      }),
    );
  }

  const reqWithToken = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(reqWithToken).pipe(
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
