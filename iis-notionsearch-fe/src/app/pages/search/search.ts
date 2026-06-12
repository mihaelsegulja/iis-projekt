import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NotionService } from '../../services/notion.service';
import { AuthService } from '../../services/auth.service';
import { NotionObject } from '../../models/models';
import { DataGridComponent, GridColumn, GridAction } from '../../shared/data-grid/data-grid';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    DataGridComponent,
  ],
  templateUrl: 'search.html',
  styleUrl: 'search.scss',
})
export class SearchPage implements OnInit {
  private readonly notion = inject(NotionService);
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  query = '';
  results = signal<NotionObject[]>([]);
  loading = signal(false);

  readonly columns: GridColumn<NotionObject>[] = [
    { field: 'icon', header: 'Icon', width: '50px' },
    { field: 'title', header: 'Title' },
    { field: 'createdTime', header: 'Created', width: '180px' },
    { field: 'lastEditedTime', header: 'Last Edited', width: '180px' },
    { field: 'inTrash', header: 'Trash', width: '70px' },
  ];

  readonly actions: GridAction<NotionObject>[] = [
    {
      icon: 'edit',
      label: 'Edit',
      onClick: (row) => this.router.navigate(['/edit', row.notionId]),
    },
    {
      icon: 'delete',
      label: 'Delete',
      color: 'warn',
      visible: () => this.auth.isAdmin(),
      onClick: (row) => this.deletePage(row),
    },
  ];

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.loading.set(true);
    this.notion.search(this.query).subscribe({
      next: (res) => {
        this.results.set(res.data ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  navigateTo(path: string): void {
    this.router.navigateByUrl(path);
  }

  private deletePage(row: NotionObject): void {
    if (!confirm(`Delete "${row.title}"?`)) return;
    this.notion.deletePage(row.notionId).subscribe({
      next: () => this.search(),
    });
  }
}
