import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-power',
  imports: [],
  templateUrl: './power.component.html',
  styleUrl: './power.component.css',
})
export class PowerComponent {
  imagesUrl = environment.imagesUrl;
}
