import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormValidationComponent} from './form-validation/form-validation.component';
import {AlertsComponent} from './alerts/alerts.component';


@NgModule({
  imports: [
    CommonModule,
    // Import standalone components so legacy modules can re-export them if needed
    AlertsComponent,
    FormValidationComponent
  ],
  exports: [
    FormValidationComponent,
    AlertsComponent
  ]
})
export class SharedModule {
}
