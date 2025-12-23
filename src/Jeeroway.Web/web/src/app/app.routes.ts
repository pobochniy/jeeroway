import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'home/env',
    loadComponent: () => import('./home/show-env/show-env.component').then(m => m.ShowEnvComponent)
  },
  {
    path: 'profiles',
    loadChildren: () => import('./profiles/profiles.module').then(m => m.routes)
  },
  {
    path: 'robo',
    loadChildren: () => import('./robo-management/robo-management-routing.module').then(m => m.routes)
  },
  {
    path: 'wiki',
    loadChildren: () => import('./wiki/wiki-routing').then(m => m.routes)
  },
  { path: '**', redirectTo: '' }
];
