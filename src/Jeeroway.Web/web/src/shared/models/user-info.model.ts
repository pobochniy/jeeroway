export class UserInfoModel {
  id: string = '';
  userName: string = '';
  email?: string;
  phone?: string;
  cash?: string;
  comment?: string;

  constructor(obj: Partial<UserInfoModel> = {}) {
    Object.assign(this, obj);
    debugger
  }
}
