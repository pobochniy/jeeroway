import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoboControlComponent } from './control/robo-control.component';
import { RoboEditComponent } from './edit/robo-edit.component';
import { RoboListComponent } from './list/robo-list.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { NgxSelectModule } from 'ngx-select-ex';
import { RoboDetailsComponent } from './details/robo-details.component';
import { RoboManagementRoutingModule } from './robo-management-routing.module';

@NgModule({
  declarations: [
    RoboControlComponent
    , RoboEditComponent
    , RoboListComponent
    , RoboDetailsComponent
  ],
  imports: [
    CommonModule
    , NgxSelectModule
    , RouterModule
    , SharedModule
    , RoboManagementRoutingModule
  ]
})
export class RoboManagementModule { }
