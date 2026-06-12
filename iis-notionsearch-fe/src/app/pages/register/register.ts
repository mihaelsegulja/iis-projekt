import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';
import { ErrorComponent } from '../../shared/error/error';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    ErrorComponent,
  ],
  templateUrl: 'register.html',
  styleUrl: 'register.scss',
})
export class RegisterPage {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.nonNullable.group(
    {
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    },
    { validators: (ctrl) => (ctrl.value.password === ctrl.value.confirmPassword ? null : { mismatch: true }) },
  );

  loading = signal(false);
  errorMessage: string | null = null;

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.errorMessage = null;
    this.auth.register({ username: this.form.value.username!, password: this.form.value.password! }).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success) this.router.navigate(['/search']);
        else this.errorMessage = res.message ?? 'Registration failed.';
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage = err.error?.message ?? 'An unexpected error occurred.';
      },
    });
  }
}
