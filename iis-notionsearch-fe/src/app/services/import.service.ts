import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StandardResponse, ImportResult } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ImportService {
  private readonly baseUrl = 'api/import';

  constructor(private http: HttpClient) {}

  importFile(file: File, contentType: string): Observable<StandardResponse<ImportResult>> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<StandardResponse<ImportResult>>(this.baseUrl, formData, {
      headers: { 'Content-Type': contentType },
    });
  }

  importRaw(body: string, contentType: string): Observable<StandardResponse<ImportResult>> {
    return this.http.post<StandardResponse<ImportResult>>(this.baseUrl, body, {
      headers: { 'Content-Type': contentType },
    });
  }
}
