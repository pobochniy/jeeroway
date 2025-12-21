import { Routes } from '@angular/router';

// Standalone routes for the Wiki feature.
// Loaded lazily from app.routes via loadChildren.
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'overwiev' },
  {
    path: 'overwiev',
    loadComponent: () => import('./overwiev/overwiev.component').then(m => m.OverwievComponent)
  },
  {
    path: 'power',
    loadComponent: () => import('./power/power.component').then(m => m.PowerComponent)
  },
  {
    path: 'rpi-install',
    loadComponent: () => import('./rpi-install/rpi-install.component').then(m => m.RpiInstallComponent)
  },
  {
    path: 'rxtx',
    loadComponent: () => import('./rxtx/rxtx.component').then(m => m.RxtxComponent)
  },
  {
    path: 'turntable',
    loadComponent: () => import('./turntable/turntable.component').then(m => m.TurntableComponent)
  }
];
