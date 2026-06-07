import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StandardResponse, WeatherResponse } from '../models/models';
import { API_URL } from '../tokens/api-url.token';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_URL)}/api/weather`;

  getWeather(cityName?: string): Observable<StandardResponse<WeatherResponse>> {
    let params = new HttpParams();
    if (cityName) params = params.set('cityName', cityName);
    return this.http.get<StandardResponse<WeatherResponse>>(this.baseUrl, { params });
  }
}
