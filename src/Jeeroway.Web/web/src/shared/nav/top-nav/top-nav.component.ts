import {ChangeDetectionStrategy, Component} from '@angular/core';
import {EventEmitterService} from '../event-emitter.service';

@Component({
  selector: 'top-nav',
  standalone: true,
  templateUrl: './top-nav.component.html',
  styleUrl: './top-nav.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TopNavComponent {
  constructor(private eventEmitterService: EventEmitterService) {}

  toggleMenu() { this.eventEmitterService.onToggleMenuButtonClick(); }
}
