import {BaseApiService} from "./base-api.service";
import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {TokenService} from "./token.service";
import {UserService} from '../services/user.service';
import {LayoutModel} from '../models/layout.model';

export interface IHomeApiService {
  Index(): Promise<LayoutModel>;
}

@Injectable()
export class HomeApiService extends BaseApiService implements IHomeApiService {
  isAuthUser = false;

  constructor(http: HttpClient, tokenService: TokenService, private userService: UserService) {
    super('Home', tokenService, http)
  }

  public async Index(): Promise<LayoutModel> {
    const res = await this.get<LayoutModel>('Index').toPromise();

    return res ?? new LayoutModel();
  }

}
