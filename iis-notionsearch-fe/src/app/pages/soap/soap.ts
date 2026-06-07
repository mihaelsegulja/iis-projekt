import { Component, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { EditorComponent } from '../../shared/editor/editor';
import { API_URL } from '../../tokens/api-url.token';

const SOAP_TEMPLATE = (term: string) => `<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
  xmlns:tem="http://tempuri.org/">
  <soap:Body>
    <tem:Search>
      <tem:term>${term}</tem:term>
    </tem:Search>
  </soap:Body>
</soap:Envelope>`;

@Component({
  selector: 'app-soap-page',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, MatFormFieldModule, MatInputModule, EditorComponent],
  template: `
    <mat-card>
      <mat-card-header><mat-card-title>SOAP / XPath</mat-card-title></mat-card-header>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search term (XPath filter)</mat-label>
          <input matInput [(ngModel)]="searchTerm" placeholder="e.g. notion" (keyup.enter)="search()" />
        </mat-form-field>
        <div class="actions">
          <button mat-raised-button color="primary" (click)="search()" [disabled]="loading">
            @if (loading) { <mat-spinner diameter="20" /> } @else { Send SOAP Request }
          </button>
        </div>

        <app-editor [(code)]="requestXml" placeholder="SOAP XML request (auto-generated)" minHeight="150px" />

        @if (errorMessage) {
          <div class="error">{{ errorMessage }}</div>
        }

        <app-editor [code]="responseXml" [isReadonly]="true" placeholder="SOAP XML response" minHeight="200px" />
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .search-field { width: 100%; margin-bottom: 8px; }
    .actions { display: flex; align-items: center; gap: 16px; margin: 12px 0; }
    .error { color: #f44336; font-size: 0.875rem; margin: 8px 0; }
  `,
})
export class SoapPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  searchTerm = '';
  requestXml = '';
  responseXml = '';
  loading = false;
  errorMessage = '';

  search(): void {
    this.requestXml = SOAP_TEMPLATE(this.searchTerm);
    this.responseXml = '';
    this.errorMessage = '';
    this.loading = true;

    this.http
      .post(`${this.apiUrl}/Soap/NotionService.asmx`, this.requestXml, {
        headers: new HttpHeaders({ 'Content-Type': 'text/xml; charset=utf-8' }),
        responseType: 'text',
      })
      .subscribe({
        next: (res) => {
          this.responseXml = typeof res === 'string' ? res : JSON.stringify(res, null, 2);
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage = err.error ?? err.message ?? 'SOAP request failed';
          this.loading = false;
        },
      });
  }
}
