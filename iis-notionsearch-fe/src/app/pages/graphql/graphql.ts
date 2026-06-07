import { Component, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EditorComponent } from '../../shared/editor/editor';
import { API_URL } from '../../tokens/api-url.token';

@Component({
  selector: 'app-graphql-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, EditorComponent],
  template: `
    <mat-card>
      <mat-card-header><mat-card-title>GraphQL Query</mat-card-title></mat-card-header>
      <mat-card-content>
        <app-editor [(code)]="query" placeholder="{ pages { title } }" minHeight="150px" />
        <div class="actions">
          <button mat-raised-button color="primary" (click)="execute()" [disabled]="loading">
            @if (loading) { <mat-spinner diameter="20" /> } @else { Execute }
          </button>
          @if (errorMessage) {
            <span class="error">{{ errorMessage }}</span>
          }
        </div>
        <app-editor [code]="response" [isReadonly]="true" placeholder="Response" />
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .actions { display: flex; align-items: center; gap: 16px; margin: 12px 0; }
    .error { color: #f44336; font-size: 0.875rem; }
  `,
})
export class GraphqlPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  query = '';
  response = '';
  loading = false;
  errorMessage = '';

  execute(): void {
    if (!this.query.trim()) return;
    this.loading = true;
    this.errorMessage = '';
    this.response = '';

    this.http
      .post<unknown>(
        `${this.apiUrl}/graphql`,
        { query: this.query },
        { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) },
      )
      .subscribe({
        next: (res) => {
          this.response = JSON.stringify(res, null, 2);
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.message ?? err.message ?? 'Request failed';
          this.loading = false;
        },
      });
  }
}
