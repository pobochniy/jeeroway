import {Injectable, Signal, signal} from "@angular/core";
import {UserInfoModel} from '../models/user-info.model';
import {MoneyModel} from '../models/money.model';
import { UserModel } from '../models/user.model';
import { TokenService } from '../api/token.service';

export interface IUserService {
  profile: Signal<UserInfoModel>;
  set(val: UserInfoModel | Partial<UserInfoModel>): void;
  persHidden: Signal<boolean>;
  setPersHidden(val: boolean): void;
}

@Injectable()
export class UserService implements IUserService {
  private readonly _profile = signal<UserInfoModel>(new UserInfoModel());
  public readonly profile = this._profile.asReadonly();
  private readonly _persHidden = signal<boolean>(false);
  public readonly persHidden = this._persHidden.asReadonly();
  /** Compatibility field used by legacy components */
  public User: UserModel | undefined;

  constructor(private tokenService: TokenService) {}

  set(val: UserInfoModel | Partial<UserInfoModel>) {
    const model = new UserInfoModel(val);
    this._profile.set(model);
  }

  setPersHidden(val: boolean) {
    this._persHidden.set(val);
  }

  /** Indicates auth state for legacy templates */
  get isAuth(): boolean { return this.tokenService.Token !== ''; }

  /** Role check forwarded to TokenService */
  hasRole(roleId: number): boolean { return this.tokenService.hasRole(roleId); }
}
