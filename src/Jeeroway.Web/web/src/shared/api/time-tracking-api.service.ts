import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {TimeTrackingModel, IUserTracking} from "../models/time-tracking.model";
import {BaseApiService} from './base-api.service';
import {FormGroup} from "@angular/forms";
import {TokenService} from './token.service';

@Injectable()
export class TimeTrackingApiService extends BaseApiService {

  constructor(http: HttpClient, tokenService: TokenService) {
    super('TimeTracking', tokenService, http)
  }

  public async Details() {
    return this.get<TimeTrackingModel>('Details').toPromise();
  }

  public async Create(model: FormGroup) {
    return this.post<TimeTrackingModel>('Create', model.value).toPromise();
  }

  public async UserTracking() {
    return this.get<IUserTracking[]>('UserTracking').toPromise();
  }
}
