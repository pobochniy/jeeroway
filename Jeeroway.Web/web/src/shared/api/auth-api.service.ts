import {BaseApiService} from "./base-api.service";
import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {TokenService} from "./token.service";
import {UserService} from '../services/user.service';

export interface IAuthApiService {
  VkAuth(queryString: object): Promise<boolean>;

  get IsAuth(): boolean;
}

@Injectable()
export class AuthApiService extends BaseApiService implements IAuthApiService {
  isAuthUser = false;

  constructor(http: HttpClient, tokenService: TokenService, private userService: UserService) {
    super('Auth', tokenService, http)
  }

  public async VkAuth(queryString: object): Promise<boolean> {
    const res = await this.post<TokenModel>('VkAuth', queryString).toPromise();
    if (res) {
      this.tokenService.Token = res.token;
      this.tokenService.Roles = res.roles ?? [];
      return (res.race ?? 0) !== 0;
    }
    return false;
  }

  public async SetName(model: any) {
    const res = await this.post<TokenModel>('SetName', model).toPromise();
    if (res) {
      this.tokenService.Token = res.token;
      this.tokenService.Roles = res.roles ?? [];
      // this.userService.Profile = new ProfileModel(res);
    }
  }

  public get IsAuth() {
    return this.tokenService.Token !== '';
  }

  /** Added for standalone Login/Register pages */
  public async login(model: any): Promise<any> {
    const body = model?.value ?? model;
    const res = await this.post<TokenModel>('Login', body).toPromise();
    if (res) {
      this.tokenService.Token = res.token;
      this.tokenService.Roles = res.roles ?? [];
      return { id: res.userId, userName: res.userName };
    }
    return null;
  }

  /** Added for standalone Login/Register pages */
  public async register(model: any): Promise<any> {
    const body = model?.value ?? model;
    const res = await this.post<TokenModel>('Register', body).toPromise();
    if (res) {
      this.tokenService.Token = res.token;
      this.tokenService.Roles = res.roles ?? [];
      return { id: res.userId, userName: res.userName };
    }
    return null;
  }

  /** Clear token locally and try to notify API (best-effort) */
  public async logOut(): Promise<void> {
    try { await this.post<void>('LogOut', {}).toPromise(); } catch {}
    this.tokenService.Token = '';
    this.tokenService.Roles = [];
  }
}

export class TokenModel {
  token: string = "";
  userId: number | null = null;
  userName: string | null = null;
  race: number = 0;
  roles: number[] = [];
}
