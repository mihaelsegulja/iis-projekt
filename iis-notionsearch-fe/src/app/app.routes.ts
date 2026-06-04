import { Routes } from '@angular/router';
import { Layout } from './layout/layout';

export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      { path: '', redirectTo: 'search', pathMatch: 'full' },
      {
        path: 'search',
        loadComponent: () => import('./pages/search/search').then((m) => m.SearchPage),
      },
      {
        path: 'new',
        loadComponent: () => import('./pages/page-form/page-form').then((m) => m.PageFormPage),
      },
      {
        path: 'weather',
        loadComponent: () => import('./pages/weather/weather').then((m) => m.WeatherPage),
      },
      {
        path: 'import',
        loadComponent: () => import('./pages/import/import').then((m) => m.ImportPage),
      },
      {
        path: 'graphql',
        loadComponent: () => import('./pages/graphql/graphql').then((m) => m.GraphqlPage),
      },
      {
        path: 'soap',
        loadComponent: () => import('./pages/soap/soap').then((m) => m.SoapPage),
      },
      {
        path: ':id',
        loadComponent: () => import('./pages/page-form/page-form').then((m) => m.PageFormPage),
      },
    ],
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then((m) => m.RegisterPage),
  },
  { path: '**', redirectTo: 'search' },
];
