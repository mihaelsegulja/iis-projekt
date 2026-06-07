import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-weather-page',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: 'weather.html',
})
export class WeatherPage {}
