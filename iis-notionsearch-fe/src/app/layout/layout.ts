import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
  ],
  template: `
    <mat-drawer-container class="layout-container">
      <mat-drawer #drawer mode="side" opened class="sidenav">
        <mat-toolbar class="sidenav-header">Menu</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item routerLink="/search" routerLinkActive="active-link">
            <mat-icon matListItemIcon>search</mat-icon>
            <span matListItemTitle>Notion Search</span>
          </a>
          <a mat-list-item routerLink="/new" routerLinkActive="active-link">
            <mat-icon matListItemIcon>add_circle</mat-icon>
            <span matListItemTitle>Create Page</span>
          </a>
          <a mat-list-item routerLink="/weather" routerLinkActive="active-link">
            <mat-icon matListItemIcon>cloud</mat-icon>
            <span matListItemTitle>Weather</span>
          </a>
          <a mat-list-item routerLink="/import" routerLinkActive="active-link">
            <mat-icon matListItemIcon>upload_file</mat-icon>
            <span matListItemTitle>Import</span>
          </a>
          <a mat-list-item routerLink="/graphql" routerLinkActive="active-link">
            <mat-icon matListItemIcon>code</mat-icon>
            <span matListItemTitle>GraphQL</span>
          </a>
          <a mat-list-item routerLink="/soap" routerLinkActive="active-link">
            <mat-icon matListItemIcon>api</mat-icon>
            <span matListItemTitle>SOAP / XPath</span>
          </a>
        </mat-nav-list>
      </mat-drawer>

      <mat-sidenav-content class="content">
        <mat-toolbar class="toolbar">
          <button mat-icon-button (click)="drawer.toggle()" aria-label="Toggle menu">
            <mat-icon>menu</mat-icon>
          </button>
          <span class="app-title">IIS Notion Search</span>
          <span class="spacer"></span>

          @if (auth.isAuthenticated()) {
            <div class="profile">
              <mat-chip class="role-chip" [class.admin-chip]="auth.isAdmin()">
                {{ auth.isAdmin() ? 'Admin' : 'User' }}
              </mat-chip>
              <span class="username">{{ auth.username() }}</span>
              <button mat-stroked-button (click)="signOut()">Sign Out</button>
            </div>
          } @else {
            <button mat-stroked-button routerLink="/login">Login</button>
          }
        </mat-toolbar>

        <main class="main">
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-drawer-container>
  `,
  styles: `
    .layout-container { height: 100vh; }
    .sidenav { width: 240px; }
    .sidenav-header { font-size: 1.1rem; font-weight: 500; }
    .active-link { background: rgba(0,0,0,0.06); }
    .content { display: flex; flex-direction: column; min-height: 100vh; }
    .toolbar { gap: 8px; }
    .app-title { font-weight: 500; font-size: 1.15rem; }
    .spacer { flex: 1 1 auto; }
    .profile { display: flex; align-items: center; gap: 12px; }
    .role-chip { font-size: 0.75rem; font-weight: 500; }
    .admin-chip { background: #ff9800 !important; color: #fff !important; }
    .username { font-size: 0.9rem; }
    .main { flex: 1; padding: 24px; }
  `,
})
export class Layout implements OnInit {
  readonly auth = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/login']);
    }
  }

  signOut() {
    this.auth.signOut().subscribe();
  }
}
