import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { EditorComponent } from '../../shared/editor/editor';
import { ErrorComponent } from '../../shared/error/error';
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
  imports: [FormsModule, MatCardModule, MatButtonModule, MatProgressSpinnerModule, MatFormFieldModule, MatInputModule, EditorComponent, ErrorComponent],
  templateUrl: 'soap.html',
  styleUrl: 'soap.scss',
})
export class SoapPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  searchTerm = '';
  requestXml = '';
  responseXml = '';
  loading = signal(false);
  errorMessage = '';

  search(): void {
    this.requestXml = SOAP_TEMPLATE(this.searchTerm);
    this.responseXml = '';
    this.errorMessage = '';
    this.loading.set(true);

    this.http
      .post(`${this.apiUrl}/Soap/NotionService.asmx`, this.requestXml, {
        headers: new HttpHeaders({ 'Content-Type': 'text/xml; charset=utf-8' }),
        responseType: 'text',
      })
      .subscribe({
        next: (res) => {
          this.responseXml = typeof res === 'string' ? res : JSON.stringify(res, null, 2);
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage = err.error ?? err.message ?? 'SOAP request failed';
          this.loading.set(false);
        },
      });
  }
}
