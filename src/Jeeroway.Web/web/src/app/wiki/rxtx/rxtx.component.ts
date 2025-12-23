import { Component } from '@angular/core';
import { environment } from '../../../environments/environment';
import { SurveyPrompt } from '../../../shared/survey-prompt/survey-prompt';

@Component({
  selector: 'app-rxtx',
  imports: [SurveyPrompt],
  templateUrl: './rxtx.component.html',
  styleUrl: './rxtx.component.css',
})
export class RxtxComponent {
  imagesUrl = environment.imagesUrl;
}
