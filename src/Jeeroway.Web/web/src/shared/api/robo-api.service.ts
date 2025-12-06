import { HttpClient } from "@angular/common/http";
import { Injectable } from '@angular/core';
import { FormGroup } from "@angular/forms";
import { RoboModel } from "../models/robo.model";
import { BaseApiService } from "./base-api.service";
import { TokenService } from "./token.service";
import { firstValueFrom } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class RoboApiService extends BaseApiService {

  constructor(tokenService: TokenService, http: HttpClient) {
    super('Robo', tokenService, http);
  }

  public List(): Promise<RoboModel[]> {
    return firstValueFrom(this.get<RoboModel[]>('List'));
  }

  public Details(roboId: string): Promise<RoboModel> {
    return firstValueFrom(this.get<RoboModel>('Details?roboId=' + roboId));
  }

  public Update(model: FormGroup): Promise<RoboModel> {
    return firstValueFrom(this.post<RoboModel>('Update', model.value));
  }

  public Delete(roboId: string): Promise<void> {
    return firstValueFrom(this.get<void>('Delete?roboId=' + roboId));
  }

}
