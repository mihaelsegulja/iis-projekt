import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { WeatherService } from '../../services/weather.service';
import { CityWeather } from '../../models/models';

@Component({
  selector: 'app-weather-page',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
  ],
  template: `
    <mat-card>
      <mat-card-content>
        <div class="toolbar-row">
          <mat-form-field appearance="outline" subscriptSizing="dynamic">
            <mat-label>Search city</mat-label>
            <input matInput [(ngModel)]="query" placeholder="e.g. Zagreb" (keyup.enter)="search()" />
          </mat-form-field>
          <button mat-raised-button color="primary" (click)="search()" [disabled]="loading()">
            <mat-icon>search</mat-icon> Search
          </button>

          @if (lastUpdated) {
            <span class="last-updated">Last updated: {{ lastUpdated }}</span>
          }
        </div>

        @if (loading()) {
          <div class="spinner-row"><mat-spinner diameter="32" /></div>
        }

        <table mat-table [dataSource]="results()" class="full-width">
          @for (col of columns; track col.field) {
            <ng-container [matColumnDef]="col.field">
              <th mat-header-cell *matHeaderCellDef> {{ col.header }} </th>
              <td mat-cell *matCellDef="let row"> {{ row[col.field] }} </td>
            </ng-container>
          }
          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>

        @if (!results().length && !loading()) {
          <div class="empty-message">No results.</div>
        }
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
    .last-updated {
      font-size: 0.9rem;
      margin-left: auto;
    }
    .full-width { width: 100%; }
    .empty-message {
      text-align: center;
      padding: 32px;
      color: rgba(0,0,0,0.5);
      font-size: 0.9rem;
    }
  `,
})
export class WeatherPage implements OnInit {
  private readonly weather = inject(WeatherService);

  query = '';
  results = signal<CityWeather[]>([]);
  loading = signal(false);
  lastUpdated = '';

  readonly columns = [
    { field: 'cityName', header: 'City' },
    { field: 'temperature', header: 'Temperature' },
    { field: 'humidity', header: 'Humidity' },
    { field: 'condition', header: 'Condition' },
    { field: 'windSpeed', header: 'Wind Speed' },
    { field: 'windDirection', header: 'Wind Direction' },
  ];

  readonly displayedColumns = this.columns.map((c) => c.field);

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.loading.set(true);
    this.weather.getWeather(this.query).subscribe({
      next: (res) => {
        this.results.set(res.data?.results ?? []);
        this.lastUpdated = res.data?.lastUpdated ?? '';
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
