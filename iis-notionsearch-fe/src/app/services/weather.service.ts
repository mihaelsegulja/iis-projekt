import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StandardResponse, WeatherResponse } from '../models/models';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private readonly baseUrl = 'api/weather';

  constructor(private http: HttpClient) {}

  getWeather(cityName?: string): Observable<StandardResponse<WeatherResponse>> {
    let params = new HttpParams();
    if (cityName) params = params.set('cityName', cityName);
    return this.http.get<StandardResponse<WeatherResponse>>(this.baseUrl, { params });
  }
}
