import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { EditorComponent } from '../../shared/editor/editor';
import { ErrorComponent } from '../../shared/error/error';
import { AuthService } from '../../services/auth.service';
import { API_URL } from '../../tokens/api-url.token';

const SNIPPETS: Record<string, string> = {
  search: `query {
  searchPages(query: "test") {
    notionId title url icon cover createdTime lastEditedTime inTrash
  }
}`,
  get: `query {
  getPage(id: "enter-page-id") {
    notionId title url icon cover createdTime lastEditedTime inTrash
  }
}`,
  create: `mutation {
  createPage(input: { title: "GraphQL Page", icon: "📄", cover: "https://example.com/cover.jpg" }) {
    notionId title url
  }
}`,
  update: `mutation {
  updatePage(id: "enter-page-id", input: { title: "Updated Title", icon: "✏️" }) {
    notionId title url
  }
}`,
  delete: `mutation {
  deletePage(id: "enter-page-id")
}`,
};

@Component({
  selector: 'app-graphql-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, MatIconModule, EditorComponent, ErrorComponent],
  templateUrl: 'graphql.html',
  styleUrl: 'graphql.scss',
})
export class GraphqlPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);
  readonly auth = inject(AuthService);

  query = '';
  response = '';
  loading = signal(false);
  errorMessage = '';

  loadSnippet(key: string): void {
    this.query = SNIPPETS[key] ?? '';
    this.response = '';
    this.errorMessage = '';
  }

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
