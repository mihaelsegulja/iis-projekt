import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { NotionService } from '../../services/notion.service';
import { NotionObject } from '../../models/models';
import { EmojiPickerComponent } from '../../shared/emoji-picker/emoji-picker';

@Component({
  selector: 'app-page-form-page',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    DatePipe,
    EmojiPickerComponent,
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ isEdit ? 'Edit Page' : 'Create Page' }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        @if (loading()) {
          <div class="spinner-row"><mat-spinner diameter="32" /></div>
        }

        @if (errorMessage) {
          <div class="error">{{ errorMessage }}</div>
        }

        <form #form="ngForm" (ngSubmit)="submit()" class="page-form">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Title</mat-label>
            <input matInput [(ngModel)]="title" name="title" required #titleModel="ngModel" />
            @if (titleModel.invalid && titleModel.touched) {
              <mat-error>Title is required</mat-error>
            }
          </mat-form-field>

          <div class="icon-row">
            <app-emoji-picker [(value)]="icon" />
            <mat-form-field appearance="outline" class="icon-url-field">
              <mat-label>or image URL</mat-label>
              <input matInput [(ngModel)]="icon" name="icon" placeholder="https://..." />
            </mat-form-field>
          </div>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Cover image URL</mat-label>
            <input matInput [(ngModel)]="cover" name="cover" placeholder="https://..." />
          </mat-form-field>

          @if (isEdit && page) {
            <mat-divider class="section-divider" />
            <div class="meta-grid">
              <div><strong>ID:</strong> {{ page.notionId }}</div>
              <div><strong>URL:</strong> <a [href]="page.url" target="_blank">{{ page.url }}</a></div>
              <div><strong>Created:</strong> {{ page.createdTime | date: 'medium' }}</div>
              <div><strong>Last Edited:</strong> {{ page.lastEditedTime | date: 'medium' }}</div>
              <div><strong>In Trash:</strong> {{ page.inTrash ? 'Yes' : 'No' }}</div>
            </div>
          }

          <div class="form-actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || submitting()">
              {{ isEdit ? 'Save Changes' : 'Create Page' }}
            </button>
            <button mat-stroked-button type="button" (click)="cancel()">Cancel</button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .spinner-row { display: flex; justify-content: center; padding: 24px; }
    .error { color: #f44336; font-size: 0.875rem; margin: 8px 0; }
    .page-form { display: flex; flex-direction: column; gap: 16px; margin-top: 16px; }
    .full-width { width: 100%; }
    .icon-row {
      display: flex;
      align-items: flex-start;
      gap: 12px;
    }
    .icon-url-field {
      flex: 1;
    }
    .section-divider { margin: 8px 0; }
    .meta-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; font-size: 0.875rem; }
    .meta-grid a { color: var(--mat-sys-primary); }
    .form-actions { display: flex; gap: 12px; margin-top: 8px; }
  `,
})
export class PageFormPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly notion = inject(NotionService);

  isEdit = false;
  pageId: string | null = null;
  page: NotionObject | null = null;

  title = '';
  icon = '';
  cover = '';
  loading = signal(false);
  submitting = signal(false);
  errorMessage = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.pageId = id;
      this.loadPage(id);
    }
  }

  private loadPage(id: string): void {
    this.loading.set(true);
    this.notion.getPage(id).subscribe({
      next: (res) => {
        if (res.data) {
          this.page = res.data;
          this.title = res.data.title;
          this.icon = res.data.icon ?? '';
          this.cover = res.data.cover ?? '';
        } else {
          this.errorMessage = res.message ?? 'Page not found';
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage = err.error?.message ?? err.message ?? 'Failed to load page';
        this.loading.set(false);
      },
    });
  }

  submit(): void {
    if (!this.title.trim()) return;
    this.submitting.set(true);
    this.errorMessage = '';

    const dto = {
      title: this.title.trim(),
      icon: this.icon || null,
      cover: this.cover || null,
    };

    const request = this.isEdit && this.pageId
      ? this.notion.updatePage(this.pageId, dto)
      : this.notion.createPage(dto);

    request.subscribe({
      next: () => this.router.navigate(['/search']),
      error: (err) => {
        this.errorMessage = err.error?.message ?? err.message ?? 'Operation failed';
        this.submitting.set(false);
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/search']);
  }
}
