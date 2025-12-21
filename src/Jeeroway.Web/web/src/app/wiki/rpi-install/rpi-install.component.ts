import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-rpi-install',
  imports: [],
  templateUrl: './rpi-install.component.html',
  styleUrl: './rpi-install.component.css',
})
export class RpiInstallComponent {
  imagesUrl = environment.imagesUrl;
}
