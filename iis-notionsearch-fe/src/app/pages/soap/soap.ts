import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-soap-page',
  standalone: true,
  imports: [MatCardModule],
  template: `<mat-card><mat-card-content><p>SOAP / XPath — coming soon</p></mat-card-content></mat-card>`,
})
export class SoapPage {}
