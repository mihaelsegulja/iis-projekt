import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: 'search.html',
})
export class SearchPage {}
