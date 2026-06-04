import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-weather-page',
  standalone: true,
  imports: [MatCardModule],
  template: `<mat-card><mat-card-content><p>Weather — coming soon</p></mat-card-content></mat-card>`,
})
export class WeatherPage {}
