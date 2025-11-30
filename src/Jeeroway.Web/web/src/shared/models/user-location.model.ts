export class UserLocationModel {
  x?: number;
  y?: number;
  roomId?: string;
  constructor(init?: Partial<UserLocationModel>) {
    Object.assign(this, init);
  }
}
