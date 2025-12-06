import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NavMenuComponent } from './menu/menu.component';
import { TopNavComponent } from './top-nav/top-nav.component';

@NgModule({
  // No declarations for standalone components; we only import and re-export them for
  // backwards compatibility with any NgModule-based consumers outside this app.
  imports: [CommonModule, RouterModule, NavMenuComponent, TopNavComponent],
  exports: [NavMenuComponent, TopNavComponent]
})
export class NavModule { }
