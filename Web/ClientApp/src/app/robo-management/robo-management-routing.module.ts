import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoboControlComponent } from './control/robo-control.component';
import { RoboDetailsComponent } from './details/robo-details.component';
import { RoboEditComponent } from './edit/robo-edit.component';
import { RoboListComponent } from './list/robo-list.component';

const routes: Routes = [{
  path: 'robo', children: [
    { path: '', redirectTo: 'list', pathMatch: 'full' },
    { path: 'control', component: RoboControlComponent },
    { path: 'edit/:id', component: RoboEditComponent },
    { path: 'details/:id', component: RoboDetailsComponent },
    { path: 'list', component: RoboListComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forRoot(routes)]
})
export class RoboManagementRoutingModule { }
