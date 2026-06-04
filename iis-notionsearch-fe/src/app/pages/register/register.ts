import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';

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
  ],
  template: `
    <div class="auth-container">
      <mat-card class="auth-card">
        <mat-card-header><mat-card-title>Create Account</mat-card-title></mat-card-header>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Username</mat-label>
              <input matInput formControlName="username" autocomplete="username" />
              @if (form.get('username')?.hasError('required') && form.get('username')?.touched) {
                <mat-error>Username is required</mat-error>
              }
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput type="password" formControlName="password" autocomplete="new-password" />
              @if (form.get('password')?.hasError('required') && form.get('password')?.touched) {
                <mat-error>Password is required</mat-error>
              }
              @if (form.get('password')?.hasError('minlength') && form.get('password')?.touched) {
                <mat-error>At least 6 characters</mat-error>
              }
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Confirm Password</mat-label>
              <input matInput type="password" formControlName="confirmPassword" autocomplete="new-password" />
              @if (form.get('confirmPassword')?.hasError('required') && form.get('confirmPassword')?.touched) {
                <mat-error>Confirm your password</mat-error>
              }
            </mat-form-field>
            @if (form.hasError('mismatch')) {
              <div class="error-message">Passwords do not match</div>
            }
            @if (errorMessage) {
              <div class="error-message">{{ errorMessage }}</div>
            }
            <button mat-raised-button color="primary" type="submit" class="full-width" [disabled]="form.invalid || loading">
              @if (loading) { <mat-spinner diameter="20" /> } @else { Register }
            </button>
          </form>
        </mat-card-content>
        <mat-card-actions><a mat-button routerLink="/login" color="primary">Already have an account? Sign In</a></mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: `
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 100vh; }
    .auth-card { width: 100%; max-width: 400px; padding: 16px; }
    .full-width { width: 100%; }
    .error-message { color: #f44336; font-size: 0.875rem; margin-bottom: 16px; }
  `,
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

  loading = false;
  errorMessage: string | null = null;

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.errorMessage = null;
    this.auth.register({ username: this.form.value.username!, password: this.form.value.password! }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) this.router.navigate(['/search']);
        else this.errorMessage = res.message ?? 'Registration failed.';
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'An unexpected error occurred.';
      },
    });
  }
}
