import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
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
  imports: [FormsModule, MatCardModule, MatButtonModule, MatCheckboxModule, MatProgressSpinnerModule, MatFormFieldModule, MatInputModule, EditorComponent, ErrorComponent],
  templateUrl: 'soap.html',
  styleUrl: 'soap.scss',
})
export class SoapPage {
  private http = inject(HttpClient);
  private apiUrl = inject(API_URL);

  searchTerm = '//NotionObjectDto';
  responseXml = '';
  simplifiedResponseXml = '';
  loading = signal(false);
  errorMessage = '';
  simplifiedView = signal(true);

  xpathSnippets = [
    { label: 'All Items', xpath: '//NotionObjectDto' },
    { label: 'By Title', xpath: "//NotionObjectDto[contains(Title, 'VALUE')]" },
    { label: 'Title + Icon', xpath: '//NotionObjectDto/Title | //NotionObjectDto/Icon' },
    { label: 'With Cover -> Title, Cover, Icon', xpath: "//NotionObjectDto[Cover]/Title | //NotionObjectDto[Cover]/Cover | //NotionObjectDto[Cover]/Icon" },
    { label: 'Pages Only', xpath: "//NotionObjectDto[ObjectType='page']" },
    { label: 'Trashed', xpath: "//NotionObjectDto[InTrash='true']" },
    { label: 'Not Trashed', xpath: "//NotionObjectDto[not(InTrash='true')]" },
    { label: 'With Icon', xpath: '//NotionObjectDto[Icon]' },
  ];

  applySnippet(xpath: string): void {
    this.searchTerm = xpath;
  }

  private extractSimplified(xml: string): string {
    const parser = new DOMParser();
    const doc = parser.parseFromString(xml, 'text/xml');
    const results = doc.evaluate(
      '//*[local-name()="Results"]',
      doc, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null
    ).singleNodeValue;
    if (results) {
      return new XMLSerializer().serializeToString(results);
    }
    return '';
  }

  search(): void {
    this.responseXml = '';
    this.simplifiedResponseXml = '';
    this.errorMessage = '';
    this.loading.set(true);

    const body = SOAP_TEMPLATE(this.searchTerm);

    this.http
      .post(`${this.apiUrl}/Soap/NotionService.asmx`, body, {
        headers: new HttpHeaders({ 'Content-Type': 'text/xml; charset=utf-8' }),
        responseType: 'text',
      })
      .subscribe({
        next: (res) => {
          const text = typeof res === 'string' ? res : JSON.stringify(res, null, 2);
          this.responseXml = text;
          this.simplifiedResponseXml = this.extractSimplified(text);
          this.loading.set(false);
        },
        error: (err) => {
          this.errorMessage = err.error ?? err.message ?? 'SOAP request failed';
          this.loading.set(false);
        },
      });
  }
}
