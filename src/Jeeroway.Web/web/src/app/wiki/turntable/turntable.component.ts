import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-turntable',
  imports: [],
  templateUrl: './turntable.component.html',
  styleUrl: './turntable.component.css',
})
export class TurntableComponent {
  imagesUrl = environment.imagesUrl;
}
