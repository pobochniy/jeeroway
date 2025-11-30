import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {BaseApiService} from './base-api.service';
import {FormGroup} from "@angular/forms";
import {TokenService} from './token.service';

@Injectable()
export class ClosePeriodApiService extends BaseApiService {

  constructor(http: HttpClient, tokenService: TokenService) {
    super('ClosePeriod', tokenService, http)
  }

  public async Calculate(model: FormGroup) {
    return this.post('Calculate', model.value).toPromise();
  }

  // public async GetList() {
  //   return this.get<ICrystalProfitPeriod[]>('GetList').toPromise();
  // }
}
