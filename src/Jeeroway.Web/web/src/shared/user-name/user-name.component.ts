import { Component, input, signal, effect, ChangeDetectionStrategy } from '@angular/core';
import { UsersApiService } from '../api/users-api.service';
import { UserProfileModel } from '../models/user-profile.model';

@Component({
  selector: 'shared-user-name',
  templateUrl: './user-name.component.html',
  styleUrls: ['./user-name.component.css'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserNameComponent {
  userId = input.required<string>();
  user = signal<UserProfileModel | null>(null);

  constructor(public users: UsersApiService) {
    effect(async () => {
      const id = this.userId();
      if (id) {
        const userData = await this.users.getUser(id);
        this.user.set(userData);
      }
    });
  }
}
