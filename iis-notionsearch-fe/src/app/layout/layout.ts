import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
  ],
  templateUrl: 'layout.html',
  styleUrl: 'layout.scss',
})
export class Layout {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    setTimeout(() => {
      if (!this.auth.isAuthenticated()) {
        this.router.navigateByUrl('/login');
      }
    });
  }

  signOut() {
    this.auth.signOut();
  }
}
