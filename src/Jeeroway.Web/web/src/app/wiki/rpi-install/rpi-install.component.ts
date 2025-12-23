import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';
import { SurveyPrompt } from '../../../shared/survey-prompt/survey-prompt';

@Component({
  selector: 'app-rpi-install',
  imports: [SurveyPrompt],
  templateUrl: './rpi-install.component.html',
  styleUrl: './rpi-install.component.css',
})
export class RpiInstallComponent {
  imagesUrl = environment.imagesUrl;
}
