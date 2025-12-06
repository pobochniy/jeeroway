import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {FormGroup} from '@angular/forms';
import {BaseApiService} from "./base-api.service";
import {HourlyPayModel} from "../models/hourly-pay.model";
import {TokenService} from './token.service';


@Injectable({ providedIn: 'root' })
export class HourlyPayApiService extends BaseApiService {
  constructor(http: HttpClient, tokenService: TokenService) {
    super('HourlyPay', tokenService, http)
  }

  public async GetList(userId: string) {
    return this.get<HourlyPayModel[]>(`GetList?userId=${userId}`).toPromise();
  }

  public async Create(model: FormGroup) {
    return this.post<number>('Create', model.value).toPromise();
  }
}
