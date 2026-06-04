import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-import-page',
  standalone: true,
  imports: [MatCardModule],
  template: `<mat-card><mat-card-content><p>Import — coming soon</p></mat-card-content></mat-card>`,
})
export class ImportPage {}
