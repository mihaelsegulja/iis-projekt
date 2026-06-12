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
  templateUrl: 'page-form.html',
  styleUrl: 'page-form.scss',
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
