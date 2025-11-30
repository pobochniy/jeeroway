import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'form-validation',
  standalone: true,
  imports: [AsyncPipe],
  templateUrl: './form-validation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FormValidationComponent {
  @Input() model!: AbstractControl;
  @Input() fieldName: string = '';
}
