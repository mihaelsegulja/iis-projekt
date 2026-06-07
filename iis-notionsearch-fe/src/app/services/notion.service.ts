import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateNotionPageDto,
  NotionObject,
  StandardResponse,
  UpdateNotionPageDto,
} from '../models/models';
import { API_URL } from '../tokens/api-url.token';

@Injectable({ providedIn: 'root' })
export class NotionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_URL)}/api/notion`;

  search(query?: string | null): Observable<StandardResponse<NotionObject[]>> {
    let params = new HttpParams();
    if (query) params = params.set('query', query);
    return this.http.get<StandardResponse<NotionObject[]>>(`${this.baseUrl}/search`, { params });
  }

  getPage(id: string): Observable<StandardResponse<NotionObject>> {
    return this.http.get<StandardResponse<NotionObject>>(`${this.baseUrl}/pages/${id}`);
  }

  createPage(dto: CreateNotionPageDto): Observable<StandardResponse<NotionObject>> {
    return this.http.post<StandardResponse<NotionObject>>(`${this.baseUrl}/pages`, dto);
  }

  updatePage(
    id: string,
    dto: UpdateNotionPageDto,
  ): Observable<StandardResponse<NotionObject>> {
    return this.http.patch<StandardResponse<NotionObject>>(`${this.baseUrl}/pages/${id}`, dto);
  }

  deletePage(id: string): Observable<StandardResponse<boolean>> {
    return this.http.delete<StandardResponse<boolean>>(`${this.baseUrl}/pages/${id}`);
  }
}
