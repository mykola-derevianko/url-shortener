import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/short-urls/short-urls-page/short-urls-page.component').then(
        (m) => m.ShortUrlsPageComponent,
      ),
  },
  {
    path: 'short-urls/:id',
    loadComponent: () =>
      import('./features/short-urls/short-url-detail/short-url-detail.component').then(
        (m) => m.ShortUrlDetailComponent,
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
