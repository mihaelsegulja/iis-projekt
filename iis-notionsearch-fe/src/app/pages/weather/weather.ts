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
  templateUrl: 'weather.html',
  styleUrl: 'weather.scss',
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
