import {Injectable} from "@angular/core";

export interface ITokenService {
  get Token();
  set Token(val: string);
  get Roles();
  set Roles(val: number[]);
  hasRole(roleId: number): boolean
}

@Injectable()
export class TokenService implements ITokenService {
  private _token: string = '';
  private _roles: number[] = [];

  public get Token() {
    if (!this._token) {
      this._token = localStorage.getItem("token") || '';
    }
    return this._token;
  }

  public set Token(val) {
    this._token = val;
    localStorage.setItem("token", val);
  }

  public get Roles(): number[] {
    return this._roles;
  }

  public set Roles(val: number[]) {
    this._roles = val;
  }

  hasRole(roleId: number): boolean {
    if (this.Token.length == 0) return false;

    return this.Roles.indexOf(roleId) > -1;
  }
}
