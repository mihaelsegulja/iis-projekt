import { HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/api/auth/')) return next(req);

  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === HttpStatusCode.Unauthorized) {
        auth.handleUnauthorized();
      }
      return throwError(() => err);
    }),
  );
};
