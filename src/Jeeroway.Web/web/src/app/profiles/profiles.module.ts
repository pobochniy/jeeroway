import { Routes } from '@angular/router';

// Standalone routes for the Profiles feature.
// Loaded lazily from app.routes via loadChildren.
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'user' },
  {
    path: 'user',
    loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent)
  },
  {
    path: 'user/:userId',
    loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent)
  }
];
