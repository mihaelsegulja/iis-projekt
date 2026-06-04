import { HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

const SKIP = ['api/auth/refresh'];

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  if (SKIP.some((p) => req.url.includes(p))) return next(req);

  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === HttpStatusCode.Unauthorized) {
        auth.refresh().subscribe({ error: () => auth.signOut().subscribe() });
      }
      return throwError(() => err);
    }),
  );
};
