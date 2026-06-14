import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DataGridComponent, GridColumn } from '../../shared/data-grid/data-grid';
import { WeatherService } from '../../services/weather.service';
import { CityWeather } from '../../models/models';

@Component({
  selector: 'app-weather-page',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    DataGridComponent,
  ],
  templateUrl: 'weather.html',
  styleUrl: 'weather.scss',
})
export class WeatherPage implements OnInit {
  private readonly weather = inject(WeatherService);

  query = '';
  lastUpdated = '';

  columns: GridColumn<CityWeather>[] = [
    { field: 'cityName', header: 'City' },
    { field: 'temperature', header: 'Temperature' },
    { field: 'humidity', header: 'Humidity' },
    { field: 'condition', header: 'Condition' },
    { field: 'windSpeed', header: 'Wind Speed' },
    { field: 'windDirection', header: 'Wind Direction' },
  ];

  data: CityWeather[] = [];
  loading = signal(false);

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.loading.set(true);
    this.weather.getWeather(this.query).subscribe({
      next: (res) => {
        this.data = res.data?.results ?? [];
        this.lastUpdated = res.data?.lastUpdated ?? '';
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
