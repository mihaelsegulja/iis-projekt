import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { EditorComponent } from '../../shared/editor/editor';
import { StandardResponse, NotionObject } from '../../models/models';
import { API_URL } from '../../tokens/api-url.token';

const JSON_SNIPPETS: Record<string, string> = {
  basic: JSON.stringify(
    { objectType: 'page', title: 'My Page', notionId: 'abc-123', url: 'https://notion.so/abc-123' },
    null,
    2,
  ),
  full: `{
  "notionId": "abc-123-def-456",
  "objectType": "page",
  "title": "Welcome to Notion",
  "url": "https://www.notion.so/Welcome-to-Notion-abc123def456",
  "icon": "📄",
  "cover": "https://images.unsplash.com/photo-1506905925346-21bda4d32df4",
  "createdTime": "2025-01-15T10:30:00Z",
  "lastEditedTime": "2025-06-10T14:22:00Z",
  "inTrash": false
}`,
  batch: `[
  {
    "notionId": "batch-001",
    "objectType": "page",
    "title": "Batch Page 1",
    "url": "https://notion.so/batch-001",
    "icon": "1️⃣"
  },
  {
    "notionId": "batch-002",
    "objectType": "page",
    "title": "Batch Page 2",
    "url": "https://notion.so/batch-002",
    "icon": "2️⃣"
  },
  {
    "notionId": "batch-003",
    "objectType": "database",
    "title": "Batch DB 3",
    "url": "https://notion.so/batch-003",
    "icon": "3️⃣"
  }
]`,
};

const XML_SNIPPETS: Record<string, string> = {
  basic: `<?xml version="1.0" encoding="utf-8"?>
<NotionObjectDto>
  <NotionId>abc-123</NotionId>
  <ObjectType>page</ObjectType>
  <Title>My Page</Title>
  <Url>https://notion.so/abc-123</Url>
</NotionObjectDto>`,
  full: `<?xml version="1.0" encoding="utf-8"?>
<NotionObjectDto>
  <NotionId>abc-123-def-456</NotionId>
  <ObjectType>page</ObjectType>
  <Title>Welcome to Notion</Title>
  <Url>https://www.notion.so/Welcome-to-Notion-abc123def456</Url>
  <Icon>📄</Icon>
  <Cover>https://images.unsplash.com/photo-1506905925346-21bda4d32df4</Cover>
  <CreatedTime>2025-01-15T10:30:00Z</CreatedTime>
  <LastEditedTime>2025-06-10T14:22:00Z</LastEditedTime>
  <InTrash>false</InTrash>
</NotionObjectDto>`,
  batch: `<?xml version="1.0" encoding="utf-8"?>
<ArrayOfNotionObjectDto>
  <NotionObjectDto>
    <NotionId>batch-001</NotionId>
    <ObjectType>page</ObjectType>
    <Title>Batch Page 1</Title>
    <Url>https://notion.so/batch-001</Url>
    <Icon>1️⃣</Icon>
  </NotionObjectDto>
  <NotionObjectDto>
    <NotionId>batch-002</NotionId>
    <ObjectType>page</ObjectType>
    <Title>Batch Page 2</Title>
    <Url>https://notion.so/batch-002</Url>
    <Icon>2️⃣</Icon>
  </NotionObjectDto>
  <NotionObjectDto>
    <NotionId>batch-003</NotionId>
    <ObjectType>database</ObjectType>
    <Title>Batch DB 3</Title>
    <Url>https://notion.so/batch-003</Url>
    <Icon>3️⃣</Icon>
  </NotionObjectDto>
</ArrayOfNotionObjectDto>`,
};

interface SnippetButton {
  key: string;
  label: string;
  icon: string;
}

const JSON_BUTTONS: SnippetButton[] = [
  { key: 'basic', label: 'Basic Page', icon: 'description' },
  { key: 'full', label: 'Full Page', icon: 'article' },
  { key: 'batch', label: 'Batch (3)', icon: 'dataset' },
];

const XML_BUTTONS: SnippetButton[] = [
  { key: 'basic', label: 'Basic Page', icon: 'description' },
  { key: 'full', label: 'Full Page', icon: 'article' },
  { key: 'batch', label: 'Batch (3)', icon: 'dataset' },
];

@Component({
  selector: 'app-import-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, MatRadioModule, MatIconModule, EditorComponent],
  templateUrl: 'import.html',
  styleUrl: 'import.scss',
})
export class ImportPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  contentType = 'application/json';
  content = '';
  fileName = '';
  loading = signal(false);
  result: { successCount: number; errorCount: number; errors: string[] | null } | null = null;

  get snippetButtons(): SnippetButton[] {
    return this.contentType === 'application/json' ? JSON_BUTTONS : XML_BUTTONS;
  }

  get placeholderText(): string {
    return this.contentType === 'application/json'
      ? '{\n  "objectType": "page",\n  "title": "My Page"\n}'
      : '<?xml version="1.0" encoding="utf-8"?>\n<NotionObjectDto>\n  ...\n</NotionObjectDto>';
  }

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

  loadSnippet(key: string): void {
    this.result = null;
    this.content =
      this.contentType === 'application/json' ? (JSON_SNIPPETS[key] ?? '') : (XML_SNIPPETS[key] ?? '');
  }

  importContent(): void {
    if (!this.content.trim()) return;
    this.loading.set(true);
    this.result = null;

    this.http
      .post<StandardResponse<NotionObject[]>>(
        `${this.apiUrl}/api/import`,
        this.content,
        { headers: new HttpHeaders({ 'Content-Type': this.contentType }) },
      )
      .subscribe({
        next: (res) => {
          const imported = res.data ?? [];
          this.result = {
            successCount: imported.length,
            errorCount: 0,
            errors: res.errors?.length ? [...res.errors] : null,
          };
          this.loading.set(false);
        },
        error: (err) => {
          const body = err.error as StandardResponse<unknown> | undefined;
          const errors: string[] = body?.errors?.length
            ? [...body.errors]
            : [body?.message ?? err.message ?? 'Request failed'];
          this.result = { successCount: 0, errorCount: errors.length, errors };
          this.loading.set(false);
        },
      });
  }
}
