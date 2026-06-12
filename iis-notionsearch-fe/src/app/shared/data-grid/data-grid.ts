import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

export interface GridColumn<T = any> {
  field: string;
  header: string;
  width?: string;
  format?: (value: any, row: T) => string;
}

export interface GridAction<T = any> {
  icon: string;
  label: string;
  onClick: (row: T) => void;
  visible?: (row: T) => boolean;
  color?: string;
}

@Component({
  selector: 'app-data-grid',
  standalone: true,
  imports: [
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  template: `
    <mat-card>
      <mat-card-content>
        <div class="toolbar-row">
          <ng-content select="[grid-toolbar]" />
        </div>

        @if (loading) {
          <div class="spinner-row">
            <mat-spinner diameter="32" />
          </div>
        }

        <div class="table-wrapper" [class.loading]="loading">
          <table mat-table [dataSource]="data" class="full-width">
            @for (col of columns; track col.field) {
              <ng-container [matColumnDef]="col.field">
                <th mat-header-cell *matHeaderCellDef [style.width]="col.width">
                  {{ col.header }}
                </th>
                <td mat-cell *matCellDef="let row" [style.width]="col.width">
                  {{ col.format ? col.format(row[col.field], row) : row[col.field] }}
                </td>
              </ng-container>
            }

            @if (actions.length) {
              <ng-container matColumnDef="actions">
                <th mat-header-cell *matHeaderCellDef style="width:140px;text-align:right">
                  Actions
                </th>
                <td mat-cell *matCellDef="let row" style="text-align:right;white-space:nowrap;overflow:visible">
                  @for (action of actions; track action.label) {
                    @if (!action.visible || action.visible(row)) {
                      <button
                        mat-icon-button
                        [color]="action.color ?? 'primary'"
                        [matTooltip]="action.label"
                        (click)="action.onClick(row)"
                      >
                        <mat-icon [fontIcon]="action.icon"></mat-icon>
                      </button>
                    }
                  }
                </td>
              </ng-container>
            }

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns" [class.empty]="!data.length"></tr>
          </table>

          @if (!data.length && !loading) {
            <div class="empty-message">No items to display.</div>
          }
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .toolbar-row {
      display: flex;
      align-items: center;
      gap: 12px;
      flex-wrap: wrap;
      padding: 8px 0 16px;
    }
    .spinner-row {
      display: flex;
      justify-content: center;
      padding: 24px;
    }
    .table-wrapper.loading {
      opacity: 0.5;
      pointer-events: none;
    }
    .full-width {
      width: 100%;
    }
    .empty-message {
      text-align: center;
      padding: 32px;
      color: rgba(0,0,0,0.5);
      font-size: 0.9rem;
    }
    tr.empty td {
      padding: 0 !important;
    }
  `,
})
export class DataGridComponent<T extends Record<string, any>> implements OnChanges {
  @Input() columns: GridColumn<T>[] = [];
  @Input() data: T[] = [];
  @Input() loading = false;
  @Input() actions: GridAction<T>[] = [];

  displayedColumns: string[] = [];

  private updateColumns(): void {
    this.displayedColumns = [
      ...this.columns.map((c) => c.field),
      ...(this.actions.length ? ['actions'] : []),
    ];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['columns'] || changes['actions'] || (changes['data'] && changes['data'].firstChange)) {
      this.updateColumns();
    }
  }
}
