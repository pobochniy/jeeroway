import {ChangeDetectionStrategy, Component, OnInit, signal} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {UserService} from '../../services/user.service';
import {UserRoleEnum} from '../../enums/user-role.enum';
import {EventEmitterService} from '../event-emitter.service';

@Component({
  selector: 'shared-nav-menu',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavMenuComponent implements OnInit {
  constructor(private eventEmitterService: EventEmitterService,
              public userService: UserService
  ) {
  }

  public roles = UserRoleEnum;

  ngOnInit(): void {
    if (this.eventEmitterService.subsMenu == undefined) {
      this.eventEmitterService.subsMenu = this.eventEmitterService
        .invokeMenuToggleMenuFunction.subscribe(() => {
          this.toggleMenu();
        });
    }
  }

  // Local UI state uses Angular signals for zoneless + OnPush compatibility
  isExpanded = signal(false);

  toggleMenu() {
    this.isExpanded.update(v => !v);
  }
}
