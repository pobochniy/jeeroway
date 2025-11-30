import {Component, OnInit, ViewChild} from '@angular/core';
import {UsersApiService} from "../../shared/api/users-api.service";
import {UserService} from "../../../shared/services/user.service";
import {UserProfileModel} from "../../../shared/models/user-profile.model";
import {HourlyPayPopupComponent} from "../hourly-pay-popup/hourly-pay-popup.component";
import {ActivatedRoute} from "@angular/router";
import {UserRoleEnum} from "../../../shared/enums/user-role.enum";


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  @ViewChild(HourlyPayPopupComponent) hourlyPayPopup!: HourlyPayPopupComponent;

  public profiles: UserProfileModel[] = [];
  public currentUser: UserProfileModel | undefined;
  public roles = UserRoleEnum;

  constructor(private route: ActivatedRoute,
              public userService: UserService,
              private profilesService: UsersApiService) {
  }

  dateAsString(date: Date): string {
    return (date + '').substring(0, 10);
  }

  async ngOnInit() {
    const that = this;
    this.route
      .params
      .subscribe(async (evt) => {
        const id = evt['userId'] || '';
        that.profiles = await that.profilesService.GetProfiles() || [];
        that.currentUser = that.profiles
          .filter(x => x.id == (id !== '' ? id : that.userService.User?.id))
          .at(0);
      })
  }
}
