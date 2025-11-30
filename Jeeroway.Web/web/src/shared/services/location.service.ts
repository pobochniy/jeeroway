import {Injectable, Signal, signal} from '@angular/core';
import {UserLocationModel} from '../models/user-location.model';
import {RoomEnum} from '../enums/room.enum';
import {LocationApiService} from '../api/location-api.service';
import {HomeApiService} from '../api/home-api.service';

@Injectable({ providedIn: 'root' })
export class LocationService {
  private readonly _location = signal<UserLocationModel>(new UserLocationModel());
  public readonly location: Signal<UserLocationModel> = this._location.asReadonly();

  constructor(
    private readonly locationApi: LocationApiService,
    private readonly homeApi: HomeApiService,
  ) {}

  set(val: UserLocationModel | Partial<UserLocationModel>) {
    const model = Object.assign(new UserLocationModel(), val);
    this._location.set(model);
  }

  async gotoRoom(room: RoomEnum): Promise<void> {
    const location = await this.locationApi.room(room);
    if (location) {
      this.set(location);
    }
  }

}
