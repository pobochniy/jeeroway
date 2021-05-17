import { HttpClient } from "@angular/common/http";
import { Injectable } from '@angular/core';
import { FormGroup } from "@angular/forms";
import { RoboModel } from "../models/robo.model";
import { BaseApiService } from "./base-api.service";


@Injectable()
export class RoboApiService extends BaseApiService {

  constructor(public http: HttpClient) {
    super('Robo', http);
  }

  public async List() {
    return this.get<RoboModel[]>('List').toPromise();
  }

  public async Details(roboId: string) {
    return this.get<RoboModel>('Details/' + roboId).toPromise();
  }

  public async Update(model: FormGroup) {
    return this.post('Update', model.value).toPromise();
  }

  public async Delete(roboId: string) {
    return this.post('Delete').toPromise();
  }

}
