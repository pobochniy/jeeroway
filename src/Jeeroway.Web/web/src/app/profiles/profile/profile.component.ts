import {Component, OnInit, ViewChild} from '@angular/core';
import {UserService} from "../../../shared/services/user.service";
import {UserProfileModel} from "../../../shared/models/user-profile.model";
import {HourlyPayPopupComponent} from "../hourly-pay-popup/hourly-pay-popup.component";
import {ActivatedRoute, RouterLink, ParamMap} from "@angular/router";
import {UserRoleEnum} from "../../../shared/enums/user-role.enum";
import {CommonModule} from '@angular/common';
import {UsersApiService} from '../../../shared/api/users-api.service';
import { ChangeDetectionStrategy, Signal, computed, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, HourlyPayPopupComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileComponent implements OnInit {

  @ViewChild(HourlyPayPopupComponent) hourlyPayPopup!: HourlyPayPopupComponent;

  public profiles = signal<UserProfileModel[]>([]);
  /** Route params as signal to react on navigation without Zone.js */
  private routeParams!: Signal<ParamMap>;
  /** Selected user id: from route or fallback to current profile id */
  public readonly selectedUserId!: Signal<string>;
  /** Current user derived from profiles and selectedUserId */
  public readonly currentUser!: Signal<UserProfileModel | undefined>;
  public roles = UserRoleEnum;

  constructor(private route: ActivatedRoute,
              public userService: UserService,
              private profilesService: UsersApiService) {
    // Initialize signals that depend on injected route
    this.routeParams = toSignal(this.route.paramMap, { initialValue: this.route.snapshot.paramMap });
    this.selectedUserId = computed<string>(() => {
      const fromRoute = this.routeParams().get('userId');
      if (fromRoute) return fromRoute;
      const profileId = this.userService.profile().id;
      if (profileId) return profileId;
      const legacyId = this.userService.User?.id;
      return legacyId != null ? String(legacyId) : '';
    });
    this.currentUser = computed<UserProfileModel | undefined>(() => {
      const list = this.profiles();
      const id = this.selectedUserId();
      return list.find(x => x.id === id) ?? list[0];
    });
  }

  dateAsString(date: Date): string {
    return (date + '').substring(0, 10);
  }

  async ngOnInit() {
    const list = await this.profilesService.GetProfiles() || [];
    this.profiles.set(list);
  }
}

