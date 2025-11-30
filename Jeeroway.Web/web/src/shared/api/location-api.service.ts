import {Injectable} from '@angular/core';
import {BaseApiService} from './base-api.service';
import {HttpClient} from '@angular/common/http';
import {TokenService} from './token.service';
import {RoomEnum} from '../enums/room.enum';
import {UserLocationModel} from '../models/user-location.model';

export interface ILocationApiService {
  room(room: RoomEnum): Promise<UserLocationModel>;
}

@Injectable()
export class LocationApiService extends BaseApiService implements ILocationApiService {
  constructor(http: HttpClient, tokenService: TokenService) {
    super('Locations', tokenService, http);
  }

  public async room(room: RoomEnum): Promise<UserLocationModel> {
    const res = await this.post<UserLocationModel>('Room', {perehod: room}).toPromise();
    return res ?? new UserLocationModel();
  }
}
