import { Component, input } from '@angular/core';

@Component({
  selector: 'app-survey-prompt',
  standalone: true,
  imports: [],
  templateUrl: './survey-prompt.html',
  styleUrl: './survey-prompt.css',
})
export class SurveyPrompt {
  title = input.required<string>();
  text = input.required<string>();
}
