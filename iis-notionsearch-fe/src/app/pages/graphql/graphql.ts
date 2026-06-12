import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EditorComponent } from '../../shared/editor/editor';
import { ErrorComponent } from '../../shared/error/error';
import { API_URL } from '../../tokens/api-url.token';

@Component({
  selector: 'app-graphql-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, EditorComponent, ErrorComponent],
  templateUrl: 'graphql.html',
  styleUrl: 'graphql.scss',
})
export class GraphqlPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  query = '';
  response = '';
  loading = signal(false);
  errorMessage = '';

  execute(): void {
    if (!this.query.trim()) return;
    this.loading.set(true);
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
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage = err.error?.message ?? err.message ?? 'Request failed';
          this.loading.set(false);
        },
      });
  }
}
