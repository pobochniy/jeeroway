import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-show-env',
  imports: [CommonModule, RouterLink],
  templateUrl: './show-env.component.html',
  styleUrl: './show-env.component.css',
})
export class ShowEnvComponent {
  env = environment;
  envEntries = Object.entries(environment);
}
