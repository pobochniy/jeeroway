import {Injectable, Signal, signal} from "@angular/core";
import {IUserService} from './user.service';
import {UserInfoModel} from '../models/user-info.model';

@Injectable()
export class UserServiceMock implements IUserService {
  private readonly _profile = signal<UserInfoModel>(new UserInfoModel());
  public readonly profile: Signal<UserInfoModel> = this._profile.asReadonly();
  private readonly _persHidden = signal<boolean>(false);
  public readonly persHidden: Signal<boolean> = this._persHidden.asReadonly();

  set(val: UserInfoModel | Partial<UserInfoModel>) {
    const model = new UserInfoModel(val);
    this._profile.set(model);
  }

  setPersHidden(val: boolean) {
    this._persHidden.set(val);
  }
}
