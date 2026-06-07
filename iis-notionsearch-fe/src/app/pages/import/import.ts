import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-import-page',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: 'import.html',
})
export class ImportPage {}
