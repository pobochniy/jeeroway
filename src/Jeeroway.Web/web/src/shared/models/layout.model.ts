import {UserInfoModel} from './user-info.model';
import {MoneyModel} from './money.model';
import {UserLocationModel} from './user-location.model';

export class LayoutModel {
  userInfo: UserInfoModel = new UserInfoModel();

  bState: string | null = null;

  moneyDto: MoneyModel = new MoneyModel();

  // public DungeonState dState;
  location: UserLocationModel | null = null;
}
