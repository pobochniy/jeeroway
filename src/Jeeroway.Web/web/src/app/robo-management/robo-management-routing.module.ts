import { Routes } from '@angular/router';

// Standalone routes for the Robo Management feature.
// Loaded lazily from app.routes via loadChildren.
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'list' },
  {
    path: 'control/:id',
    loadComponent: () => import('./control/robo-control.component').then(m => m.RoboControlComponent)
  },
  {
    path: 'edit',
    loadComponent: () => import('./edit/robo-edit.component').then(m => m.RoboEditComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./edit/robo-edit.component').then(m => m.RoboEditComponent)
  },
  {
    path: 'details/:id',
    loadComponent: () => import('./details/robo-details.component').then(m => m.RoboDetailsComponent)
  },
  {
    path: 'list',
    loadComponent: () => import('./list/robo-list.component').then(m => m.RoboListComponent)
  }
];
