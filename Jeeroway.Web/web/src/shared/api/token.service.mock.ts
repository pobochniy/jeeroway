import {Injectable} from "@angular/core";
import {ITokenService} from './token.service';

@Injectable()
export class TokenServiceMock implements ITokenService {
  private _roles: number[] = [1];

  get Roles(): number[] {
    return this._roles;
  }

  set Roles(val: number[]) {
    this._roles = val;
  }

  /** Список ролей */
  public roles: number[] = [];

  public get Token() {
    return '111';
  }

  public set Token(val) {
  }

  hasRole(roleId: number): boolean {
    return this.Roles.indexOf(roleId) > -1;
  }
}
