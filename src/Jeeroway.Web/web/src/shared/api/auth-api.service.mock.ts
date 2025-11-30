import {Injectable} from "@angular/core";
import {IAuthApiService, TokenModel} from './auth-api.service';
import {TokenService} from './token.service';
import {HttpClient} from '@angular/common/http';

@Injectable()
export class AuthApiServiceMock implements IAuthApiService {

  private hasRace = false;

  constructor(
    private tokenService: TokenService
  ) {
  }

  /**
   * Configure the mock to return a specific auth state.
   */
  public SetupAuth(val: boolean) { this.hasRace = val; }

  public async VkAuth(queryString: object): Promise<boolean> {
    // Simulate minimal behavior
    const res = new TokenModel();
    res.token = "";
    res.race = 0;
    return this.hasRace;
  }

  public get IsAuth() {
    return this.tokenService.Token !== '';
  }
}

