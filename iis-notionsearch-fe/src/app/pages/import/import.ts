import { Component, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { EditorComponent } from '../../shared/editor/editor';
import { API_URL } from '../../tokens/api-url.token';

const SNIPPETS: Record<string, string> = {
  json: '{\n  "objectType": "page",\n  "title": "My Page"\n}',
  xml: '<?xml version="1.0" encoding="utf-8"?>\n<NotionObject>\n  <ObjectType>page</ObjectType>\n  <Title>My Page</Title>\n</NotionObject>',
};

@Component({
  selector: 'app-import-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, MatRadioModule, EditorComponent],
  templateUrl: 'import.html',
  styleUrl: 'import.scss',
})
export class ImportPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  contentType = 'application/json';
  content = '';
  fileName = '';
  loading = false;
  result: { successCount: number; errorCount: number; errors: string[] | null } | null = null;

  onFileSelect(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.fileName = file.name;
    const reader = new FileReader();
    reader.onload = () => {
      this.content = reader.result as string;
      this.contentType = file.name.endsWith('.xml') ? 'application/xml' : 'application/json';
    };
    reader.readAsText(file);
  }

  loadSnippet(): void {
    this.content = SNIPPETS[this.contentType === 'application/json' ? 'json' : 'xml'];
  }

  importContent(): void {
    if (!this.content.trim()) return;
    this.loading = true;
    this.result = null;

    this.http
      .post<{ success: boolean; data: { successCount: number; errorCount: number; errors: string[] | null } | null }>(
        `${this.apiUrl}/api/import`,
        this.content,
        { headers: new HttpHeaders({ 'Content-Type': this.contentType }) },
      )
      .subscribe({
        next: (res) => {
          this.result = res.data ?? { successCount: 0, errorCount: 0, errors: ['No data returned'] };
          this.loading = false;
        },
        error: (err) => {
          this.result = { successCount: 0, errorCount: 1, errors: [err.error?.message ?? err.message ?? 'Request failed'] };
          this.loading = false;
        },
      });
  }
}
