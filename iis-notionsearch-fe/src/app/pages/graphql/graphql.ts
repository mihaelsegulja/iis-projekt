import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-graphql-page',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: 'graphql.html',
})
export class GraphqlPage {}
